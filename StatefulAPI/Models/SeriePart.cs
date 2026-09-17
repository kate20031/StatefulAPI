using System;

namespace StatefulAPI.Models.EntityFramework;

public partial class Serie
{
    public override bool Equals(object? obj)
    {
        if (obj is not Serie other)
            return false;



        return Serieid == other.Serieid &&
               Titre == other.Titre &&
               Resume  == other.Resume &&
               Nbsaisons == other.Nbsaisons &&
               Nbepisodes  == other.Nbepisodes &&
               Anneecreation == other.Anneecreation &&
               Network == other.Network;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Serieid,
            Titre,
             Resume,
            Nbsaisons,
            Nbepisodes,
            Anneecreation,
            Network
        );
    }
}