Remove-Item -Recurse "output"
New-Item -ItemType Directory -Force "output"
$path = (Get-Item -Path ".\").FullName
$env:COR_ENABLE_PROFILING = "1"
$env:COR_PROFILER = "{cf0d821e-299b-5307-a3d8-9ccb4916d2e5}"
$env:COR_PROFILER_PATH_32 = "C:\build\bin\latest\cs_dotnet_tracer_32.dll"
$env:COR_PROFILER_PATH_64 = "C:\build\bin\latest\cs_dotnet_tracer_64.dll"
$env:CS_OUTPUT_DIR = "$path\output"
echo $env:CS_OUTPUT_DIR
./bin/Debug/FrameworkSample.exe
Remove-Item Env:\COR_ENABLE_PROFILING
Remove-Item Env:\COR_PROFILER
Remove-Item Env:\COR_PROFILER_PATH_32
Remove-Item Env:\COR_PROFILER_PATH_64
Remove-Item Env:\CS_OUTPUT_DIR
