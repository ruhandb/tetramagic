using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardSpotMap
{
    private IDictionary<string, NeighborSpots> _spots = new Dictionary<string, NeighborSpots>
    {
        { "A1", new NeighborSpots{ t = null, b = "B1", l = null, r = "A2"} },
        { "A2", new NeighborSpots{ t = null, b = "B2", l = "A1", r = "A3"} },
        { "A3", new NeighborSpots{ t = null, b = "B3", l = "A2", r = "A4"} },
        { "A4", new NeighborSpots{ t = null, b = "B4", l = "A3", r = null} },

        { "B1", new NeighborSpots{ t = "A1", b = "C1", l = null, r = "B2"} },
        { "B2", new NeighborSpots{ t = "A2", b = "C2", l = "B1", r = "B3"} },
        { "B3", new NeighborSpots{ t = "A3", b = "C3", l = "B2", r = "B4"} },
        { "B4", new NeighborSpots{ t = "A4", b = "C4", l = "B3", r = null} },

        { "C1", new NeighborSpots{ t = "B1", b = "D1", l = null, r = "C2"} },
        { "C2", new NeighborSpots{ t = "B2", b = "D2", l = "C1", r = "C3"} },
        { "C3", new NeighborSpots{ t = "B3", b = "D3", l = "C2", r = "C4"} },
        { "C4", new NeighborSpots{ t = "B4", b = "D4", l = "C3", r = null} },

        { "D1", new NeighborSpots{ t = "C1", b = null, l = null, r = "D2"} },
        { "D2", new NeighborSpots{ t = "C2", b = null, l = "D1", r = "D3"} },
        { "D3", new NeighborSpots{ t = "C3", b = null, l = "D2", r = "D4"} },
        { "D4", new NeighborSpots{ t = "C4", b = null, l = "D3", r = null} },
    };

    public NeighborSpots this[string key]
    { get
        {
            return _spots[key];
        }
    }
}

public class NeighborSpots
{
    public string t;
    public string b;
    public string r;
    public string l;
}
