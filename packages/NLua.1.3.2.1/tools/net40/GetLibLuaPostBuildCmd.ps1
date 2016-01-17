
$nativePath = $(Join-Path $installPath "lib\native\*.*")
$LibLuaPostBuildCmd =  "
xcopy /s /y `"$nativePath`" `"`$(TargetDir)`""