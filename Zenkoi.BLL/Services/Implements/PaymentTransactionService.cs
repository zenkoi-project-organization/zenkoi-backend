using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.PaymentTransactionDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<PaymentTransaction> _transactionRepo;

        public PaymentTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _transactionRepo = _unitOfWork.GetRepo<PaymentTransaction>();
        }

        public async Task<PaginatedList<PaymentTransactionResponseDTO>> GetAllTransactionsAsync(
            PaymentTransactionFilterDTO filter,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<PaymentTransaction>()
                .WithInclude(t => t.User)
                .WithInclude(t => t.ActualOrder)
                .WithOrderBy(q => q.OrderByDescending(t => t.CreatedAt))
                .WithTracking(false);

            // Apply filters
            ApplyFilters(queryBuilder, filter);

            var query = _transactionRepo.Get(queryBuilder.Build());
            var pagedTransactions = await PaginatedList<PaymentTransaction>.CreateAsync(query, pageIndex, pageSize);

            var resultDto = _mapper.Map<List<PaymentTransactionResponseDTO>>(pagedTransactions.ToList());

            return new PaginatedList<PaymentTransactionResponseDTO>(
                resultDto,
                pagedTransactions.TotalItems,
                pageIndex,
                pageSize
            );
        }

        public async Task<PaginatedList<PaymentTransactionResponseDTO>> GetMyTransactionsAsync(
            int userId,
            PaymentTransactionFilterDTO filter,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<PaymentTransaction>()
                .WithPredicate(t => t.UserId == userId)
                .WithInclude(t => t.User)
                .WithInclude(t => t.ActualOrder)
                .WithOrderBy(q => q.OrderByDescending(t => t.CreatedAt))
                .WithTracking(false);

            ApplyFiltersForMyTransactions(queryBuilder, filter);

            var query = _transactionRepo.Get(queryBuilder.Build());
            var pagedTransactions = await PaginatedList<PaymentTransaction>.CreateAsync(query, pageIndex, pageSize);

            var resultDto = _mapper.Map<List<PaymentTransactionResponseDTO>>(pagedTransactions.ToList());

            return new PaginatedList<PaymentTransactionResponseDTO>(
                resultDto,
                pagedTransactions.TotalItems,
                pageIndex,
                pageSize
            );
        }

        public async Task<PaymentTransactionResponseDTO> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepo.GetSingleAsync(
                new QueryBuilder<PaymentTransaction>()
                    .WithPredicate(t => t.Id == id)
                    .WithInclude(t => t.User)
                    .WithInclude(t => t.ActualOrder)
                    .WithTracking(false)
                    .Build()
            );

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy transaction với id {id}.");
            }

            return _mapper.Map<PaymentTransactionResponseDTO>(transaction);
        }

        public async Task<PaymentTransactionResponseDTO> GetTransactionByActualOrderIdAsync(int actualOrderId)
        {
            var transaction = await _transactionRepo.GetSingleAsync(
                new QueryBuilder<PaymentTransaction>()
                    .WithPredicate(t => t.ActualOrderId == actualOrderId)
                    .WithInclude(t => t.User)
                    .WithInclude(t => t.ActualOrder)
                    .WithTracking(false)
                    .Build()
            );

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy transaction cho Order ID {actualOrderId}.");
            }

            return _mapper.Map<PaymentTransactionResponseDTO>(transaction);
        }

        private void ApplyFiltersForMyTransactions(QueryBuilder<PaymentTransaction> queryBuilder, PaymentTransactionFilterDTO filter)
        {
            if (filter == null) return;         

            if (!string.IsNullOrWhiteSpace(filter.PaymentMethod))
            {
                queryBuilder.WithPredicate(t => t.PaymentMethod == filter.PaymentMethod);
            }

            if (filter.Status.HasValue)
            {
                queryBuilder.WithPredicate(t => t.Status == filter.Status.Value);
            }

            if (filter.FromDate.HasValue)
            {
                queryBuilder.WithPredicate(t => t.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                queryBuilder.WithPredicate(t => t.CreatedAt <= filter.ToDate.Value);
            }

            if (filter.MinAmount.HasValue)
            {
                queryBuilder.WithPredicate(t => t.Amount >= filter.MinAmount.Value);
            }

            if (filter.MaxAmount.HasValue)
            {
                queryBuilder.WithPredicate(t => t.Amount <= filter.MaxAmount.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.ToLower();
                queryBuilder.WithPredicate(t =>
                    t.OrderId.ToLower().Contains(keyword) ||
                    (t.TransactionId != null && t.TransactionId.ToLower().Contains(keyword)) ||
                    (t.Description != null && t.Description.ToLower().Contains(keyword))
                );
            }
        }

        private void ApplyFilters(QueryBuilder<PaymentTransaction> queryBuilder, PaymentTransactionFilterDTO filter)
        {
            if (filter == null) return;

            if (filter.UserId.HasValue)
            {
                queryBuilder.WithPredicate(t => t.UserId == filter.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.PaymentMethod))
            {
                queryBuilder.WithPredicate(t => t.PaymentMethod == filter.PaymentMethod);
            }

            if (filter.Status.HasValue)
            {
                queryBuilder.WithPredicate(t => t.Status == filter.Status.Value);
            }

            if (filter.FromDate.HasValue)
            {
                queryBuilder.WithPredicate(t => t.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                queryBuilder.WithPredicate(t => t.CreatedAt <= filter.ToDate.Value);
            }

            if (filter.MinAmount.HasValue)
            {
                queryBuilder.WithPredicate(t => t.Amount >= filter.MinAmount.Value);
            }

            if (filter.MaxAmount.HasValue)
            {
                queryBuilder.WithPredicate(t => t.Amount <= filter.MaxAmount.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.ToLower();
                queryBuilder.WithPredicate(t =>
                    t.OrderId.ToLower().Contains(keyword) ||
                    (t.TransactionId != null && t.TransactionId.ToLower().Contains(keyword)) ||
                    (t.Description != null && t.Description.ToLower().Contains(keyword))
                );
            }
        }
    }
}
