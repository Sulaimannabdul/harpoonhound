# sharphound

## Compile Instructions

```
dotnet restore.
dotnet build
```

## Requirements

SharpHound is designed targeting .Net 4.6.2. SharpHound must be run from the context of a domain user, either directly through a logon or through another method such as RUNAS.

#### ***src*** future update

```
src/
│
├──Client
   |── context.cs ✔              
   ├── enums.cs ✔
   ├── flags.cs ✔
   ├── links.cs ✔
   ├── vulnscan.cs ✔   
   |
   |───RES
      |── harpoon.cpp ✔
      |── harpoon.hpp ✔
      |── utils.cpp ✔    
            
```

#### ***sharphound.csproj*** future update

```
note: nothing now
```

![bloodhound-logo](https://github.com/Sulaimannabdul/sharphound/assets/151133481/8ff5cadd-b696-4089-9830-69446a4c28bd)
