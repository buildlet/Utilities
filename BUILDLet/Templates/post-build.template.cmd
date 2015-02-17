SET TARGET_PROJECT=BUILDLet.WOLSetup
SET TARGET_DIRECTORY=Sources
ECHO Copy "$(TargetPath)" to "$(SolutionDir)%TARGET_PROJECT%\%TARGET_DIRECTORY%"...
COPY "$(TargetPath)" "$(SolutionDir)%TARGET_PROJECT%\%TARGET_DIRECTORY%"

SET TARGET_DIRECTORY=Release
ECHO Copy "$(TargetPath)" to "$(SolutionDir)%TARGET_DIRECTORY%"...
COPY "$(TargetPath)" "$(SolutionDir)%TARGET_DIRECTORY%"
