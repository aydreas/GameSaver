# GameSaver
Tool for automatically copying game saves to a different directory - for example to save to the cloud

#### Sample Config:
Config must be placed next to the executable and must be named 'config.json'.
```
{
  "Locations": [
    {
      "Name": "TransportFever",
      "Paths": [
        {
          "Source": ">>Full path to source directory or file to copy<<",
          "Destination": ">>Full path to destination, where the contents should be copied to<<",
          "RegEx": ">>Regex expression to filter files or leave empty to copy every file<<"
        }
      ],
      "AssociatedProcess": {
        "ProcessName": ">>process name of eg. the game without extension, so the files won't be copied when the game is running<<"
      },
      "LastUpdated": ""
    }
  ],
  "RefreshInterval": 10000
}
```

## License
This Project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
