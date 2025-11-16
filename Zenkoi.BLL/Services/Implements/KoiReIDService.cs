using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiReIDDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class KoiReIDService : IKoiReIDService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<KoiIdentification> _identificationRepo;
        private readonly IRepoBase<KoiGalleryEnrollment> _enrollmentRepo;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<ApplicationUser> _userRepo;
        private readonly HttpClient _httpClient;
        private readonly string _pythonApiBaseUrl;

        public KoiReIDService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identificationRepo = _unitOfWork.GetRepo<KoiIdentification>();
            _enrollmentRepo = _unitOfWork.GetRepo<KoiGalleryEnrollment>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
            _httpClient = httpClient;
            _pythonApiBaseUrl = configuration["PythonReIDApi:BaseUrl"] ?? "http://localhost:8000";
        }

        public async Task<EnrollFromCloudinaryResponseDTO> EnrollKoiFromCloudinaryAsync(
            int koiFishId,
            List<string> cloudinaryUrls,
            int userId,
            bool overrideExisting = false)
        {
            // Validate KoiFish exists
            var koiFish = await _koiFishRepo.GetByIdAsync(koiFishId);
            if (koiFish == null)
            {
                throw new ArgumentException($"Không tìm thấy cá Koi với id {koiFishId}.");
            }

            // Generate fish_id
            var fishId = $"koi_{koiFishId}";

            // Call Python API
            var requestBody = new
            {
                fishId = fishId,
                imageUrls = cloudinaryUrls,
                @override = overrideExisting
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_pythonApiBaseUrl}/gallery/enroll-from-urls",
                    content
                );

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var pythonResponse = JsonSerializer.Deserialize<PythonEnrollResponseDTO>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (pythonResponse == null || !pythonResponse.Success)
                {
                    throw new Exception($"Python API trả về lỗi: {pythonResponse?.Message}");
                }

                // Save enrollment to database
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Deactivate old enrollments if override
                    if (overrideExisting)
                    {
                        var oldEnrollments = await _enrollmentRepo.GetAllAsync(
                            new QueryBuilder<KoiGalleryEnrollment>()
                                .WithPredicate(e => e.KoiFishId == koiFishId && e.IsActive)
                                .Build()
                        );

                        foreach (var old in oldEnrollments)
                        {
                            old.IsActive = false;
                            old.UpdatedAt = DateTime.UtcNow;
                            await _enrollmentRepo.UpdateAsync(old);
                        }
                    }

                    // Create new enrollment
                    var enrollment = new KoiGalleryEnrollment
                    {
                        KoiFishId = koiFishId,
                        FishIdInGallery = fishId,
                        NumImages = pythonResponse.NumDownloaded,
                        EnrolledAt = DateTime.UtcNow,
                        EnrolledBy = userId,
                        IsActive = true
                    };

                    await _enrollmentRepo.CreateAsync(enrollment);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return new EnrollFromCloudinaryResponseDTO
                    {
                        Success = true,
                        Message = pythonResponse.Message,
                        FishId = pythonResponse.FishId,
                        NumImages = pythonResponse.NumImages,
                        NumUrlsProvided = pythonResponse.NumUrlsProvided,
                        NumDownloaded = pythonResponse.NumDownloaded,
                        TotalFishInGallery = pythonResponse.TotalFishInGallery,
                        EnrollmentId = enrollment.Id
                    };
                }
                catch
                {
                    await _unitOfWork.RollBackAsync();
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Không thể kết nối đến Python API: {ex.Message}", ex);
            }
        }

        public async Task<KoiIdentificationResponseDTO> IdentifyKoiFromUrlAsync(
            string imageUrl,
            int userId,
            int topK = 5,
            decimal threshold = 0.15m)
        {
            // Call Python API
            var requestBody = new
            {
                imageUrl = imageUrl,
                topK = topK,
                threshold = (double)threshold
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_pythonApiBaseUrl}/identify/from-url",
                    content
                );

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                // Debug: Log raw JSON từ Python API
                Console.WriteLine($"[DEBUG] Python API Response: {jsonString}");

                var pythonResponse = JsonSerializer.Deserialize<PythonIdentifyResponseDTO>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (pythonResponse == null)
                {
                    throw new Exception("Python API trả về response không hợp lệ.");
                }

                // Debug: Log TopPredictions sau khi deserialize
                Console.WriteLine($"[DEBUG] TopPredictions count: {pythonResponse.TopPredictions?.Count ?? 0}");
                if (pythonResponse.TopPredictions != null)
                {
                    foreach (var pred in pythonResponse.TopPredictions)
                    {
                        Console.WriteLine($"[DEBUG] Prediction: FishId={pred.FishId}, Distance={pred.Distance}, Similarity={pred.Similarity}");
                    }
                }

                // Save to database
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var serializedTopPredictions = JsonSerializer.Serialize(pythonResponse.TopPredictions);
                    Console.WriteLine($"[DEBUG] Serialized TopPredictions: {serializedTopPredictions}");

                    var identification = new KoiIdentification
                    {
                        ImageUrl = imageUrl,
                        IdentifiedAs = pythonResponse.FishId,
                        Confidence = pythonResponse.Similarity,
                        Distance = pythonResponse.Distance,
                        IsUnknown = pythonResponse.IsUnknown,
                        TopPredictions = serializedTopPredictions,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    };

                    // Link với KoiFishId nếu không phải unknown
                    if (!pythonResponse.IsUnknown)
                    {
                        var enrollment = await _enrollmentRepo.GetSingleAsync(
                            new QueryBuilder<KoiGalleryEnrollment>()
                                .WithPredicate(e => e.FishIdInGallery == pythonResponse.FishId && e.IsActive)
                                .Build()
                        );

                        if (enrollment != null)
                        {
                            identification.KoiFishId = enrollment.KoiFishId;
                        }
                    }

                    await _identificationRepo.CreateAsync(identification);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return await GetIdentificationByIdAsync(identification.Id);
                }
                catch
                {
                    await _unitOfWork.RollBackAsync();
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Không thể kết nối đến Python API: {ex.Message}", ex);
            }
        }

        public async Task<PaginatedList<KoiGalleryEnrollmentResponseDTO>> GetEnrollmentsAsync(
            int? koiFishId = null,
            bool? isActive = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<KoiGalleryEnrollment>()
                .WithInclude(e => e.KoiFish)
                .WithInclude(e => e.EnrolledByUser)
                .WithOrderBy(q => q.OrderByDescending(e => e.EnrolledAt))
                .WithTracking(false);

            if (koiFishId.HasValue)
            {
                queryBuilder.WithPredicate(e => e.KoiFishId == koiFishId.Value);
            }

            if (isActive.HasValue)
            {
                queryBuilder.WithPredicate(e => e.IsActive == isActive.Value);
            }

            var query = _enrollmentRepo.Get(queryBuilder.Build());
            var pagedEnrollments = await PaginatedList<KoiGalleryEnrollment>.CreateAsync(query, pageIndex, pageSize);

            var resultDto = pagedEnrollments.Select(e => new KoiGalleryEnrollmentResponseDTO
            {
                Id = e.Id,
                KoiFishId = e.KoiFishId,
                KoiFishRFID = e.KoiFish?.RFID ?? string.Empty,
                FishIdInGallery = e.FishIdInGallery,
                NumImages = e.NumImages,
                EnrolledAt = e.EnrolledAt,
                EnrolledByName = e.EnrolledByUser?.FullName,
                IsActive = e.IsActive,
                UpdatedAt = e.UpdatedAt
            }).ToList();

            return new PaginatedList<KoiGalleryEnrollmentResponseDTO>(
                resultDto,
                pagedEnrollments.TotalItems,
                pageIndex,
                pageSize
            );
        }

        public async Task<PaginatedList<KoiIdentificationResponseDTO>> GetIdentificationHistoryAsync(
            int? koiFishId = null,
            bool? isUnknown = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<KoiIdentification>()
                .WithInclude(i => i.KoiFish)
                .WithInclude(i => i.CreatedByUser)
                .WithOrderBy(q => q.OrderByDescending(i => i.CreatedAt))
                .WithTracking(false);

            if (koiFishId.HasValue)
            {
                queryBuilder.WithPredicate(i => i.KoiFishId == koiFishId.Value);
            }

            if (isUnknown.HasValue)
            {
                queryBuilder.WithPredicate(i => i.IsUnknown == isUnknown.Value);
            }

            var query = _identificationRepo.Get(queryBuilder.Build());
            var pagedIdentifications = await PaginatedList<KoiIdentification>.CreateAsync(query, pageIndex, pageSize);

            var resultDto = pagedIdentifications.Select(i => new KoiIdentificationResponseDTO
            {
                Id = i.Id,
                KoiFishId = i.KoiFishId,
                KoiFishRFID = i.KoiFish?.RFID,
                ImageUrl = i.ImageUrl,
                IdentifiedAs = i.IdentifiedAs,
                Confidence = i.Confidence,
                Distance = i.Distance,
                IsUnknown = i.IsUnknown,
                TopPredictions = string.IsNullOrEmpty(i.TopPredictions)
                    ? null
                    : JsonSerializer.Deserialize<List<TopPredictionDTO>>(i.TopPredictions),
                CreatedAt = i.CreatedAt,
                CreatedByName = i.CreatedByUser?.FullName
            }).ToList();

            return new PaginatedList<KoiIdentificationResponseDTO>(
                resultDto,
                pagedIdentifications.TotalItems,
                pageIndex,
                pageSize
            );
        }

        public async Task<KoiIdentificationResponseDTO> GetIdentificationByIdAsync(int id)
        {
            var identification = await _identificationRepo.GetSingleAsync(
                new QueryBuilder<KoiIdentification>()
                    .WithPredicate(i => i.Id == id)
                    .WithInclude(i => i.KoiFish)
                    .WithInclude(i => i.CreatedByUser)
                    .WithTracking(false)
                    .Build()
            );

            if (identification == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy identification với id {id}.");
            }

            // Debug: Log TopPredictions từ database
            Console.WriteLine($"[DEBUG] TopPredictions from DB: {identification.TopPredictions}");

            List<TopPredictionDTO>? deserializedTopPredictions = null;
            if (!string.IsNullOrEmpty(identification.TopPredictions))
            {
                deserializedTopPredictions = JsonSerializer.Deserialize<List<TopPredictionDTO>>(identification.TopPredictions);
                Console.WriteLine($"[DEBUG] Deserialized TopPredictions count: {deserializedTopPredictions?.Count ?? 0}");
            }

            return new KoiIdentificationResponseDTO
            {
                Id = identification.Id,
                KoiFishId = identification.KoiFishId,
                KoiFishRFID = identification.KoiFish?.RFID,
                ImageUrl = identification.ImageUrl,
                IdentifiedAs = identification.IdentifiedAs,
                Confidence = identification.Confidence,
                Distance = identification.Distance,
                IsUnknown = identification.IsUnknown,
                TopPredictions = deserializedTopPredictions,
                CreatedAt = identification.CreatedAt,
                CreatedByName = identification.CreatedByUser?.FullName
            };
        }

        public async Task<bool> DeactivateEnrollmentAsync(int enrollmentId, int userId)
        {
            var enrollment = await _enrollmentRepo.GetByIdAsync(enrollmentId);
            if (enrollment == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy enrollment với id {enrollmentId}.");
            }

            enrollment.IsActive = false;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _enrollmentRepo.UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
