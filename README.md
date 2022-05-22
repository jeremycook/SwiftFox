# Swiftfox

# Development

```sh
# Install, update and use dotnet outdated to update packages
dotnet tool install --global dotnet-outdated-tool
dotnet tool update --global dotnet-outdated-tool

# List all outdated packages
dotnet-outdated

# Outdated Microsoft packages, excluding major releases
dotnet-outdated -inc Microsoft. -vl Major
dotnet-outdated -inc Microsoft. -vl Major -u

# Outdated non-Microsoft packages
dotnet-outdated -exc Microsoft.
dotnet-outdated -exc Microsoft. -u
```
