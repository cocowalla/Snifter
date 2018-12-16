Snifter
=======

<table border="0">
 <tr>
    <td>
      <pre>
      __.---,__
   .-`         '-,__
 &/           ',_\ _\
 /               '',_
 |    .            (")
 |__.`'-..--|__|--``   Snifter
      </pre>
    </td>
    <td>
      <p>Snifter is a raw socket IP packet capturing tool for Windows and Linux, with a tiny CPU and memory footprint.</p>
      <p>Output is written in <a href="https://github.com/pcapng/pcapng">PCAPNG</a> format, and you can filter captured packets based on protocol, source/destination address and source/destination port.</p>
    </td>
 </tr>
</table>

Why?
----

On Windows, you can't capture on the local loopback address `127.0.0.1` with a packet capture driver like [WinPcap](https://wiki.wireshark.org/WinPcap) - but you can by using a *raw socket* sniffer, like Snifter.

Additionally, Snifter is a cross-platform, portable tool that doesn't require any drivers to be installed.

Snifter started life as a Windows-only tool, and Linux support was later added just because .NET Core makes it possible.

Limitations
-----------

You must run Snifter with elevated privileges on Windows, or with `sudo` on Linux - this is an OS-level requirement to create raw sockets.

For now at least, Snifter only supports IPv4. It should be straightforward to add support for IPv6, but I don't use IPv6 yet, so haven't added it.

If you want to capture loopback traffic, it's important that your apps are communicating specifically with `127.0.0.1` - *not* `localhost`.

Note that Snifter is restricted to only capturing *TCP* packets on Linux.

Usage
-----

````
snifter.exe -i x -f filename

  -i, --interface=VALUE      ID of the interface to listen on
  -f, --filename=VALUE       Filename to output sniffed packets to. Defaults to snifter.pcapng
  -o, --operator=VALUE       Whether filters should be AND or OR. Defaults to OR
  -p, --protocol=VALUE       Filter packets by IANA registered protocol number
  -s, --source-address=VALUE Filter packets by source IP address
  -d, --dest-address=VALUE   Filter packets by destination IP address
  -x, --source-port=VALUE    Filter packets by source port number
  -y, --dest-port=VALUE      Filter packets by destination port number
  -h, -?, --help             Show command line options
````

Run `snifter.exe -h` to see a list of available network interfaces.

Note that each filter option can only be specified once.
