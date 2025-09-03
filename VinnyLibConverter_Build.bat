REM Step 1: Autobuild solution in Release-mode
set VSVER=[17.0^,18.0^)

::Edit path if VS 2022 is installed on other path
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvarsall.bat" x64

::Build VinnyLibConverter
devenv VinnyLibConverter.sln /Build "Release|Any CPU"

::Build nwc2swig
devenv ..\nwc2swig\nwcreateWrapper.sln /Build "Release|x64"

::Build VinnyNavisworksAdapter
devenv ..\VinnyNavisworksAdapter\VinnyNavisworksAdapter.sln /Build "R_N21|x64"

::Build VinnyRengaAdapter
devenv ..\VinnyRengaPlugin\VinnyRenga.sln /Build "Release|x64"

::ZIP release

"C:\Program Files\7-Zip\7z" a -tzip "VinnyLibConverter.zip" "..\VinnyLibBin\Release"

@endlocal
@exit /B 1
