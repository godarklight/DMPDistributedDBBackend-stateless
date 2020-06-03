#!/bin/sh
rm -rf build
rm build.zip
cd DMPDistributedDBBackend
dotnet publish -r linux-arm --no-self-contained -c Release -f netcoreapp3.1
dotnet publish -r linux-x64 --no-self-contained -c Release -f netcoreapp3.1
dotnet publish -r osx-x64 --no-self-contained -c Release -f netcoreapp3.1
dotnet publish -r win-x64 --no-self-contained -c Release -f netcoreapp3.1
cd ..
mkdir build
cp -av DMPDistributedDBBackend/bin/Release/netcoreapp3.1/linux-arm/publish/ build/linux-arm
cp -av DMPDistributedDBBackend/bin/Release/netcoreapp3.1/linux-x64/publish/ build/linux-x64
cp -av DMPDistributedDBBackend/bin/Release/netcoreapp3.1/osx-x64/publish/ build/osx-x64
cp -av DMPDistributedDBBackend/bin/Release/netcoreapp3.1/win-x64/publish/ build/win-x64
zip -r build.zip build/
