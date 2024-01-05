# mobile-hotspot

A console application to create a mobile hotspot on Windows 10.

A mobile hotspot will be created automatically using the best available connection profile, and will remain active until the application is closed.


Reads env SSID and PASSPHRASE from a file `.env`.

Dotenv file must be in the same directory as the binary file.

## Output format

The app can be called from automated scripts, and the output is designed to be easily parseable. Example output is provided here:

```
> mobilehotspot.exe
CON | 01:23:45:67:89:ab | DeviceName | 192.168.1.2
DIS | 01:23:45:67:89:ab | DeviceName | 192.168.1.2
Stopping hotspot ...
```

The start and end messages are written to stderr, the lines in the middle are written to stdout.

The general output format looks like this:

`<code> | <mac> | <host1>[ | <host2>] ...`

Where
- `<code>` is `CON` (client connected) or `DIS` (client disconnected)
- `<mac>` is the clients MAC address
- `<host1>` ... `<hostn>` are the client's hostnames (usually a local IP address and a device name)

Connected/Disconnected events are polled every 10 seconds.

## Closing the app

To close the app, press "ctrl+c". To close the app from an automated script or program.
