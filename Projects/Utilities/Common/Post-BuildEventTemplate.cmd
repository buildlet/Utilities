REM ##### Post-Build Event Template Version 1.6 #####
SET DESTINATION_DIR=$(SolutionDir)bin\$(ConfigurationName)
IF NOT EXIST "%DESTINATION_DIR%" ( MKDIR "%DESTINATION_DIR%" )
COPY "$(TargetPath)" "%DESTINATION_DIR%"
COPY "$(TargetDir)$(TargetName).xml" "%DESTINATION_DIR%"