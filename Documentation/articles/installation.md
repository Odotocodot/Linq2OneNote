# Installation
#### Step 1
First make sure you have a local installation of OneNote, such as [OneNote](https://apps.microsoft.com/detail/xpffzhvgqwwlhb?hl=en-gb&gl=GB) (previously OneNote 2016) or [OneNote for Windows 10](https://www.microsoft.com/store/productId/9WZDNCRFHVJL?ocid=pdpshare).

#### Step 2
Create a console application, or open an existing one.

#### Step 3
Install the library from NuGet [here](https://www.nuget.org/packages/Odotocodot.OneNote.Linq/) or run the following command in your project directory:
```
dotnet add package Odotocodot.OneNote.Linq
```

#### Step 4
Next code away!

#### Afterword
See [samples](samples.md) for examples on how to use the library.

> [!IMPORTANT]
> This library only works for local versions of OneNote, and does not make use of the Microsoft Graph API.

> [!IMPORTANT]
> This library is windows only as it uses COM Interop to interact with OneNote.