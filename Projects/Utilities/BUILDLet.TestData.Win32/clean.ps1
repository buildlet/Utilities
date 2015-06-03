$targets = @(
    ".\BUILDLet.Win32.sdf"
    ".\Debug"
    ".\ipch"
    ".\BUILDLet.Win32.TestDLL\Debug"
) | % {
    if (Test-Path -Path $_ -PathType Leaf)      { Remove-Item $_ -Force -Verbose }
    if (Test-Path -Path $_ -PathType Container) { Remove-Item $_ -Force -Verbose -Recurse }
}
