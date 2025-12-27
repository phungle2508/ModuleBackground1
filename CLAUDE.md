# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a BricsCAD plugin (ModuleBackground) for automated architectural drafting, specifically for foundation and structural drawing automation in Japanese construction projects. The plugin is written in C# targeting .NET Framework 4.5 and uses the BricsCAD API (BrxMgd) and Teigha Runtime.

**Target Platform:** BricsCAD (AutoCAD-compatible CAD software)
**Language:** C# (.NET Framework 4.5)
**Project Type:** Windows Forms plugin with CAD automation

## Build and Development Commands

### Building the Project
**IMPORTANT:** Do NOT build this project while BricsCAD is running. The BricsCAD application locks the plugin DLL (`ModuleBackground.dll`) while loaded, causing build failures. You must either:
- Close BricsCAD before building, OR
- Use the BricsCAD `NETUNLOAD` command to unload the plugin before building

```bash
# Build using dotnet CLI (only when BricsCAD is closed)
dotnet build ModuleBackground.csproj

# Build for release
dotnet build ModuleBackground.csproj -c Release
```

**NOTE:** Code analysis and editing can be done without building. Focus on code changes rather than compilation.

### Project File Structure
- **Source code:** `ModuleBackground/` directory
- **Output:** `bin/Debug/net45/` or `bin/Release/net45/`
- **Resources:**
  - `bin/Debug/net45/symbol/` - CAD block symbols (.dwg files)
  - `bin/Debug/net45/Ini/` - Configuration files (.ini)

### External Dependencies
The project references these CAD API assemblies (relative paths):
- `BrxMgd.dll` - BricsCAD managed API
- `TD_Mgd.dll` - Teigha Runtime managed API
- Location: `../New folder (5)/Tama Son Thanh Vietnam/TSV/`

Note: These paths may need adjustment if the dependency location changes.

## Architecture and Code Organization

### Entry Point and Command Registration
- **CMyCommand.cs** - Implements `IExtensionApplication`, the plugin entry point
  - `Initialize()` - Called when BricsCAD loads the plugin, launches the modeless FormMenu
  - All plugin commands are registered here using `[CommandMethod]` attributes
  - Commands follow naming pattern: `ST_*` (e.g., `ST_SplitRoom`, `ST_DrawGridsIntersect`)
  - Each command method: hides the menu form → locks document → executes operation → shows menu form

### Main UI
- **FormMenu.cs** - Primary modeless dialog with button grid
  - Sends commands to BricsCAD via `Document.SendStringToExecute()`
  - Buttons map to the `ST_*` commands defined in CMyCommand
  - UI text is in Japanese, reflecting the target market

### Core Drawing Classes

**Symbol Management:**
- **CSymbol.cs** - Manages CAD block insertion from DWG files
  - `CreateNewBlock()` - Imports block from DWG file into drawing database
  - `GetBlockTableId()` - Gets or creates block definition in drawing
  - `InsertBlockFromFile()` - Inserts block reference at specified point

**Entity Jig Classes (Interactive Drawing):**
- **BlockJig.cs** - Interactive block placement with mirror flip across lines
- **BlockMovingScaling.cs**, **BlockMovingScalingRotating.cs** - Block transform jigs
- **RotateJig.cs** - Interactive rotation
- **TextPlacementJig.cs** - Text positioning
- **BlockMirrorJig.cs** - Block mirroring
- **CBlockAttJig.cs** - Block attribute editing

**Feature Classes:**
- **CSplitRoom.cs** - Room splitting functionality
- **CInsSymIntersect.cs** - Symbol insertion at line intersections
- **CInsSymNearSquares.cs** - Symbol insertion near rectangular areas (anchor bolt placement)
- **CInsSymJintsuko.cs** - Specialized symbol insertion
- **CWall.cs** - Wall representation (stores two line ObjectIds and lengths)
- **CInterSectRegion.cs** - Intersection region calculations
- **CSquares.cs** - Square/rectangle detection and management
- **CDrawLine.cs** - Line drawing operations
- **CDrawPattern.cs** - Pattern/hatching
- **CDrawLeader.cs** - Leader line annotation
- **CTextNote.cs** - Text note placement
- **CInfoSurfaceCut.cs** - Surface cut data input/output
- **CInputRoomUB.cs** - Bathroom/UB room input

**Settings Forms:**
- **FormSettingBackGround.cs** - Layer/color settings
- **FormSettingInputSurfaceCut.cs** - Surface cut input settings
- **FormSettingOutSurfaceCut.cs** - Surface cut output settings
- **FormSettingSymJintsuko.cs** - Symbol settings
- **FormSettingPattern.cs** - Pattern settings
- **FormSettingTextNote.cs** - Text note settings
- **FormSettingRoomUB.cs** - Room settings

### Utilities and Helpers

- **GlobalFunction.cs** - Global static utilities
  - `InitIni()` - Loads configuration from INI file
  - `GetPathFolderBinary()`, `GetPathFolderIni()`, `GetPathFolderSymbol()` - Path resolution
  - `AppendBlockIntersect()` - Adds block to model space
  - `setOSNAP()`, `revertOSNAP()` - Object snap mode management
  - Global layer/color configuration variables
  - Default values: `g_dTextHieght = 125.0`, `g_nFullOsnap = 16383`

- **CIniFile.cs** - INI file wrapper using Windows API
  - P/Invoke to `kernel32.dll`: `GetPrivateProfileString`, `WritePrivateProfileString`
  - Handles comma-separated value lists for configuration

- **AlphanumComparatorFast.cs** - Alphanumeric sorting comparator

- **GetBitmapDwgFile.cs** - Extracts preview images from DWG files

- **CSettingObject.cs** - Settings management

- **CInfoObjCut.cs** - Cut object information

### Configuration System

**INI File Structure** (`bin/Debug/net45/Ini/Background.ini`):
```ini
[SystemBackground]
LineWall=LAYER_NAME,COLOR_INDEX
LineHatch=LAYER_NAME,COLOR_INDEX
LineSquare=LAYER_NAME,COLOR_INDEX
LineIntersect=LAYER_NAME,COLOR_INDEX

[InputSurfaceCut]
1=FG1_1
2=FG2
...
```

Configuration is loaded via `GlobalFunction.InitIni()` at the start of each command.

## CAD API Patterns Used

### Document Locking Pattern
All commands follow this pattern:
```csharp
HideFormMenu();
using (doc.LockDocument()) {
    GlobalFunction.InitIni();
    // Operation here
}
ShowFormMenu();
```

### Object Snap Management
```csharp
object curOsnap = 0;
GlobalFunction.setOSNAP(GlobalFunction.g_nFullOsnap, ref curOsnap);
// Drawing operations
GlobalFunction.revertOSNAP(curOsnap);
```

### Block Insertion Workflow
1. Check if block exists in drawing's BlockTable
2. If not, import from DWG file in `symbol/` folder
3. Create BlockReference at insertion point
4. Append to Model Space
5. Add to transaction for proper cleanup

### Jig Pattern for Interactive Drawing
```csharp
// Create jig entity
EntityJig jig = new MyJig(parameters);
PromptResult result = doc.Editor.Drag(jig);
if (result.Status == PromptStatus.OK) {
    // Get final position/transform and add to database
}
```

## Key Implementation Details

- **Unsafe Code:** Project allows `unsafe` blocks (`AllowUnsafeBlocks=True`)
- **Language Version:** C# 12.0
- **UI Framework:** Windows Forms (modeless dialogs that coexist with CAD interface)
- **String Resources:** Japanese UI text is embedded directly in forms
- **Symbol Files:** External DWG blocks in `symbol/common/`, `symbol/UB/`, `symbol/SurfaceCut/`
- **Layer Naming:** Uses Japanese layer names (e.g., `_0-0_デ__タ_001`)

## Common Commands (available in BricsCAD)

All commands are prefixed with `ST_` and registered under group `Nctri_Module`:
- `ST_SplitRoom` - Split rooms
- `ST_DrawGridsIntersect` - Draw intersection grids
- `ST_SettingBackground` - Configure layer/color settings
- `ST_InputSurfaceCut` - Input surface cut data
- `ST_OutputSurfaceCut` - Output surface cut drawings
- `ST_InsSymJintsuko` - Insert Jintsuko symbols
- `ST_InsSymIntersect1/2` - Insert symbols at intersections
- `ST_InsSymNearSquares1/2/3/4` - Insert anchor bolt symbols
- `ST_InputUB` - Bathroom room input
- `ST_DrawLine100` - Draw 100mm lines
- `ST_DrawLine75` - Draw 75mm lines
- `ST_DrawPattern` - Draw reinforcement patterns
- `ST_TextNote` - Add text notes
- `ST_DrawLeader` - Draw leader lines
- `ST_InputOther` - Input other data

## Development Notes

- **DO NOT BUILD while BricsCAD is running** - the DLL is locked by BricsCAD and cannot be overwritten
- The plugin integrates closely with BricsCAD's document management system
- All drawing operations must be wrapped in document locks
- The main menu (FormMenu) is modeless and persists during CAD operations
- Symbol block definitions are cached in the drawing's BlockTable for reuse
- Japanese text encoding may be relevant for INI file parsing
- The project uses relative paths for external DLL dependencies - these must exist at runtime
- After code changes, reload the plugin in BricsCAD using `NETLOAD` or restart BricsCAD
