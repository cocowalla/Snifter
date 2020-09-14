
// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
namespace Snifter.Protocol.Internet
{
    /// <summary>
    /// IP protocols, as specified in RFC 790
    /// </summary>
    public enum IpProtocol : byte
    {
        /// <summary>IPv6 Hop-by-Hop Option</summary>
        HOPOPT = 0,
        
        /// <summary>Internet Control Message Protocol</summary>
        ICMP = 1,
        
        /// <summary>Internet Group Management Protocol</summary>
        IGMP = 2,

        /// <summary>Gateway-to-Gateway Protocol</summary>
        GGP = 3,

        /// <summary>IP in IP (encapsulation)</summary>
        IP_in_IP = 4,

        /// <summary>Internet Stream Protocol</summary>
        ST = 5,

        /// <summary>Transmission Control Protocol</summary>
        TCP = 6,

        /// <summary>Core-based trees</summary>
        CBT = 7,

        /// <summary>Exterior Gateway Protocol</summary>
        EGP = 8,

        /// <summary>Interior Gateway Protocol (any private interior gateway (used by Cisco for their IGRP))</summary>
        IGP = 9,

        /// <summary>BBN RCC Monitoring</summary>
        BBN_RCC_MON = 10,

        /// <summary>Network Voice Protocol</summary>
        NVP_II = 11,

        /// <summary>Xerox PUP</summary>
        PUP = 12,

        /// <summary>ARGUS</summary>
        ARGUS = 13,

        /// <summary>EMCON</summary>
        EMCON = 14,

        /// <summary>Cross Net Debugger</summary>
        XNET = 15,

        /// <summary>Chaos</summary>
        CHAOS = 16,

        /// <summary>User Datagram Protocol</summary>
        UDP = 17,

        /// <summary>Multiplexing</summary>
        MUX = 18,

        /// <summary>DCN Measurement Subsystems</summary>
        DCN_MEAS = 19,

        /// <summary>Host Monitoring Protocol</summary>
        HMP = 20,

        /// <summary>Packet Radio Measurement</summary>
        PRM = 21,

        /// <summary>XEROX NS IDP</summary>
        XNS_IDP = 22,

        /// <summary>Trunk-1</summary>
        TRUNK_1 = 23,

        /// <summary>Trunk-2</summary>
        TRUNK_2 = 24,

        /// <summary>Leaf-1</summary>
        LEAF_1 = 25,

        /// <summary>Leaf-2</summary>
        LEAF_2 = 26,

        /// <summary>Reliable Data Protocol</summary>
        RDP = 27,

        /// <summary>Internet Reliable Transaction Protocol</summary>
        IRTP = 28,

        /// <summary>ISO Transport Protocol Class 4</summary>
        ISO_TP4 = 29,

        /// <summary>Bulk Data Transfer Protocol</summary>
        NETBLT = 30,

        /// <summary>MFE Network Services Protocol</summary>
        MFE_NSP = 31,

        /// <summary>MERIT Internodal Protocol</summary>
        MERIT_INP = 32,

        /// <summary>Datagram Congestion Control Protocol</summary>
        DCCP = 33,

        /// <summary>Third Party Connect Protocol</summary>
        ThreePC = 34,

        /// <summary>Inter-Domain Policy Routing Protocol</summary>
        IDPR = 35,

        /// <summary>Xpress Transport Protocol</summary>
        XTP = 36,

        /// <summary>Datagram Delivery Protocol</summary>
        DDP = 37,

        /// <summary>IDPR Control Message Transport Protocol</summary>
        IDPR_CMTP = 38,
        
        /// <summary>TP++ Transport Protocol</summary>
        TP_PlusPlus = 39,

        /// <summary>IL Transport Protocol</summary>
        IL = 40,

        /// <summary>IPv6 Encapsulation</summary>
        IPv6 = 41,

        /// <summary>Source Demand Routing Protocol</summary>
        SDRP = 42,

        /// <summary>Routing Header for IPv6</summary>
        IPv6_Route = 43,

        /// <summary>Fragment Header for IPv6</summary>
        IPv6_Frag = 44,

        /// <summary>Inter-Domain Routing Protocol</summary>
        IDRP = 45,

        /// <summary>Resource Reservation Protocol</summary>
        RSVP = 46,

        /// <summary>Generic Routing Encapsulation</summary>
        GREs = 47,

        /// <summary>Dynamic Source Routing Protocol</summary>
        DSR = 48,

        /// <summary>Burroughs Network Architecture</summary>
        BNA = 49,

        /// <summary>Encapsulating Security Payload</summary>
        ESP = 50,

        /// <summary>Authentication Header</summary>
        AH = 51,

        /// <summary>Integrated Net Layer Security Protocol</summary>
        I_NLSP = 52,

        /// <summary>SwIPe</summary>
        SwIPe = 53,

        /// <summary>NBMA Address Resolution Protocol</summary>
        NARP = 54,

        /// <summary>IP Mobility (Min Encap)</summary>
        MOBILE = 55,

        /// <summary>Transport Layer Security Protocol (using Kryptonet key management)</summary>
        TLSP = 56,

        /// <summary>Simple Key-Management for Internet Protocol</summary>
        SKIP = 57,

        /// <summary>ICMP for IPv6</summary>
        IPv6_ICMP = 58,

        /// <summary>No Next Header for IPv6</summary>
        IPv6_NoNxt = 59,

        /// <summary>Destination Options for IPv6</summary>
        IPv6_Opts = 60,

        /// <summary>Any host internal protocol</summary>
        HostInternal = 61,

        /// <summary>CFTP</summary>
        CFTP = 62,

        /// <summary>Any local network</summary>
        LocalNetwork = 63,

        /// <summary>SATNET and Backroom EXPAK</summary>
        SAT_EXPAK = 64,

        /// <summary>Kryptolan</summary>
        KRYPTOLAN = 65,

        /// <summary>MIT Remote Virtual Disk Protocol</summary>
        RVD = 66,

        /// <summary>Internet Pluribus Packet Core</summary>
        IPPC = 67,

        /// <summary>Any distributed file system</summary>
        DistributedFileSystem = 68,

        /// <summary>SATNET Monitoring</summary>
        SAT_MON = 69,

        /// <summary>VISA Protocol</summary>
        VISA = 70,

        /// <summary>Internet Packet Core Utility</summary>
        IPCU = 71,

        /// <summary>Computer Protocol Network Executive</summary>
        CPNX = 72,

        /// <summary>Computer Protocol Heart Beat</summary>
        CPHB = 73,

        /// <summary>Wang Span Network</summary>
        WSN = 74,

        /// <summary>Packet Video Protocol</summary>
        PVP = 75,

        /// <summary>Backroom SATNET Monitoring</summary>
        BR_SAT_MON = 76,

        /// <summary>SUN ND PROTOCOL-Temporary</summary>
        SUN_ND = 77,

        /// <summary>WIDEBAND Monitoring</summary>
        WB_MON = 78,

        /// <summary>WIDEBAND EXPAK</summary>
        WB_EXPAK = 79,

        /// <summary>International Organization for Standardization Internet Protocol</summary>
        ISO_IP = 80,

        /// <summary>Versatile Message Transaction Protocol</summary>
        VMTP = 81,

        /// <summary>Secure Versatile Message Transaction Protocol</summary>
        SECURE_VMTP = 82,

        /// <summary>VINES</summary>
        VINES = 83,

        /// <summary>TTP</summary>
        TTP = 84,

        /// <summary>Internet Protocol Traffic Manager</summary>
        IPTM = 84,

        /// <summary>NSFNET-IGP</summary>
        NSFNET_IGP = 85,

        /// <summary>Dissimilar Gateway Protocol</summary>
        DGP = 86,

        /// <summary>TCF</summary>
        TCF = 87,

        /// <summary>EIGRP</summary>
        EIGRP = 88,

        /// <summary>Open Shortest Path First</summary>
        OSPF = 89,

        /// <summary>Sprite RPC Protocol</summary>
        Sprite_RPC = 90,

        /// <summary>Locus Address Resolution Protocol</summary>
        LARP = 91,

        /// <summary>Multicast Transport Protocol</summary>
        MTP = 92,

        /// <summary>AX.25</summary>
        AX25 = 93,

        /// <summary>KA9Q NOS compatible IP over IP tunneling</summary>
        OS = 94,

        /// <summary>Mobile Internetworking Control Protocol</summary>
        MICP = 95,

        /// <summary>Semaphore Communications Sec. Pro</summary>
        SCC_SP = 96,

        /// <summary>Ethernet-within-IP Encapsulation</summary>
        ETHERIP = 97,

        /// <summary>Encapsulation Header</summary>
        ENCAP = 98,

        /// <summary>Any private encryption scheme</summary>
        PrivateEncryptionScheme = 99,

        /// <summary>GMTP</summary>
        GMTP = 100,

        /// <summary>Ipsilon Flow Management Protocol</summary>
        IFMP = 101,

        /// <summary>PNNI over IP</summary>
        PNNI = 102,

        /// <summary>Protocol Independent Multicast</summary>
        PIM = 103,

        /// <summary>IBM's ARIS (Aggregate Route IP Switching) Protocol</summary>
        ARIS = 104,

        /// <summary>SCPS (Space Communications Protocol Standards)</summary>
        SCPS = 105,

        /// <summary>QNX</summary>
        QNX = 106,

        /// <summary>Active Networks</summary>
        AN = 107,

        /// <summary>IP Payload Compression Protocol</summary>
        IPComp = 108,

        /// <summary>Sitara Networks Protocol</summary>
        SNP = 109,

        /// <summary>Compaq Peer Protocol</summary>
        Compaq_Peer = 110,

        /// <summary>IPX in IP</summary>
        IPX_in_IP = 111,

        /// <summary>Virtual Router Redundancy Protocol, Common Address Redundancy Protocol (not IANA assigned)</summary>
        VRRP = 112,

        /// <summary>PGM Reliable Transport Protocol</summary>
        PGM = 113,

        /// <summary>Any 0-hop protocol</summary>
        AnyZeroHop = 114,

        /// <summary>Layer Two Tunneling Protocol Version 3</summary>
        L2TP = 115,

        /// <summary>D-II Data Exchange (DDX)</summary>
        DDX = 116,

        /// <summary>Interactive Agent Transfer Protocol</summary>
        IATP = 117,

        /// <summary>Schedule Transfer Protocol</summary>
        STP = 118,

        /// <summary>SpectraLink Radio Protocol</summary>
        SRP = 119,

        /// <summary>Universal Transport Interface Protocol</summary>
        UTI = 120,

        /// <summary>Simple Message Protocol</summary>
        SMP = 121,

        /// <summary>Simple Multicast Protocol</summary>
        SM = 122,

        /// <summary>Performance Transparency Protocol</summary>
        PTP = 123,

        /// <summary>Intermediate System to Intermediate System (IS-IS) Protocol over IPv4</summary>
        IS_IS_over_IPv4 = 124,

        /// <summary>Flexible Intra-AS Routing Environment</summary>
        FIRE = 125,

        /// <summary>Combat Radio Transport Protocol</summary>
        CRTP = 126,

        /// <summary>Combat Radio User Datagram</summary>
        CRUDP = 127,

        /// <summary>Service-Specific Connection-Oriented Protocol in a Multi-link and Connectionless Environment</summary>
        SSCOPMCE = 128,

        /// <summary></summary>
        IPLT = 129,

        /// <summary>Secure Packet Shield</summary>
        SPS = 130,

        /// <summary>Private IP Encapsulation within IP</summary>
        PIPE = 131,

        /// <summary>Stream Control Transmission Protocol</summary>
        SCTP = 132,

        /// <summary>Fibre Channel</summary>
        FC = 133,

        /// <summary>Reservation Protocol (RSVP) End-to-End Ignore</summary>
        RSVP_E2E_IGNORE = 134,

        /// <summary>Header</summary>
        Mobility = 135,

        /// <summary>Lightweight User Datagram Protocol</summary>
        UDPLite = 136,

        /// <summary>Multi-protocol Label Switching Encapsulated in IP</summary>
        MPLS_in_IP = 137,

        /// <summary>MANET Protocols</summary>
        manet = 138,

        /// <summary>Host Identity Protocol</summary>
        HIP = 139,

        /// <summary>Site Multi-homing by IPv6 Intermediation</summary>
        Shim6 = 140,

        /// <summary>Wrapped Encapsulating Security Payload</summary>
        WESP = 141,

        /// <summary>Robust Header Compression</summary>
        ROHC = 142,

        /// <summary>IPv6 Segment Routing</summary>
        Ethernet = 143
    }
}
