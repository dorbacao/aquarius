cd ..
cd ..
cd ".nuget"

set path=%path%;%CD%

cd ..

cd package
cd Aquarius.Data.EF

nuget setApiKey 89fb5795-1537-4b82-b4af-b85b303ff8b0
nuget push output\*.nupkg