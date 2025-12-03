**BreedingProcessController Diagram**

- **File:** `docs/diagrams/BreedingProcessController.puml`
- **Purpose:** PlantUML class diagram for `BreedingProcessController` showing dependencies, public methods, and key DTOs.

**How to render (Windows, cmd.exe)**

- Option A — Using PlantUML jar (requires Java and Graphviz installed):

```cmd
REM download plantuml.jar to project root or specify path
java -jar plantuml.jar -tpng docs\diagrams\BreedingProcessController.puml
```

The command will produce `BreedingProcessController.png` in the same folder.

- Option B — Render all diagrams in the folder:

```cmd
java -jar plantuml.jar -tpng docs\diagrams\*.puml
```

- Option C — Use VS Code extension:
  - Install `PlantUML` extension and `Graphviz` on your machine.
  - Open `BreedingProcessController.puml` and use the PlantUML preview to export PNG/SVG.

**Notes**
- PlantUML requires Graphviz for layout; ensure `dot` is on PATH.
- If you prefer SVG, change `-tpng` to `-tsvg` in the commands above.

**Next steps I can take for you**
- Export PNG/SVG for you (if you allow me to run PlantUML locally here and provide the jar/Graphviz),
- Expand the diagram to include all DTO/Entity fields (I can extract DTO definitions from the repo),
- Convert this diagram to Mermaid or PlantUML sequence diagram for `Recommend` flow.

Tell me which next step you want.