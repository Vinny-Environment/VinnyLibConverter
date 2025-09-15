REM Step 1: Autobuild solution in Release-mode
set VSVER=[17.0^,18.0^)

::Edit path if VS 2022 is installed on other path
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvarsall.bat" x64

rmdir /s /q "..\VinnyLibBin\Release"


::Build nwc2swig
::Перед запуском скрипта надо вручную собрать проект nwcreateWrapperLib в составе nwc2swig (cs файлы не включаются в репозиторий, т.к. это генерируемые данные)

devenv ..\nwc2swig\nwcreateWrapper.sln /Build "Release|x64"
xcopy ..\nwc2swig\README.md "..\VinnyLibBin\Release\dependencies\nwcreate" /Y /I

::Build VinnyProjTransformation
::Перед запуском скрипта надо вручную собрать проект VinnyProjWrapperLib в составе VinnyProjTransformation (cs файлы не включаются в репозиторий, т.к. это генерируемые данные)

devenv ..\VinnyProjTransformation\VinnyProjTransformation.sln /Build "Release|x64"
xcopy ..\VinnyProjTransformation\README.md "..\VinnyLibBin\Release\dependencies\proj" /Y /I

::Build VinnyLibConverter
devenv VinnyLibConverter.sln /Build "Release|Any CPU"
xcopy .\README.md "..\VinnyLibBin\Release\" /Y /I

::Build VinnyNavisworksAdapter
devenv ..\VinnyNavisworksAdapter\VinnyNavisworksAdapter.sln /Build "R_N21|x64"
xcopy ..\VinnyNavisworksAdapter\README.md "..\VinnyLibBin\Release\plugins\navisworks" /Y /I

::Build VinnyRengaAdapter
devenv ..\VinnyRengaPlugin\VinnyRenga.sln /Build "Release|x64"
xcopy ..\VinnyRengaPlugin\README.md "..\VinnyLibBin\Release\plugins\renga" /Y /I

::Build VinnyCADLibAdapter
devenv ..\VinnyCADLibAdapter\VinnyCADLibAdapter.sln /Build "Release|x64"
xcopy ..\VinnyCADLibAdapter\README.md "..\VinnyLibBin\Release\plugins\cadlib" /Y /I

::ZIP release

"C:\Program Files\7-Zip\7z" a -tzip "VinnyLibConverter.zip" "..\VinnyLibBin\Release"

::@endlocal
::@exit /B 1
