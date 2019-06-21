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
          "Source": "C:\\Users\\Andreas\\Desktop\\src\\",
          "Destination": "C:\\Users\\Andreas\\Desktop\\srcc\\",
          "RegEx": "^setup.*$"
        }
      ],
      "AssociatedProcess": {
        "ProcessName": "putty"
      },
      "LastUpdated": "2019-06-21T18:50:27.1283765Z"
    }
  ],
  "RefreshInterval": 1000
}
```

## License
This Project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
