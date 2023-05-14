﻿using System;

/*Deck - Vine Boom
 * 
 *  Play the vine boom sound.
 * 
 */

public class CPHInline
{
    public bool Execute()
    {
        //Declarations
        string str_path, str_marker, str_media;
        float f_vol;

        //Initializations
        str_path = CPH.GetGlobalVar<string>("mediaRoot");
        f_vol = CPH.GetGlobalVar<float>("mediaVolume");
        str_marker = "『SOUNDBOARD』 " + "BOOM";
        str_media = "VineBoom.mp3";

        //If I'm live...
        if (CPH.ObsIsStreaming())
        {
            //... create a marker.
            CPH.CreateStreamMarker(str_marker);
        }//if

        CPH.PlaySound(str_path + str_media, f_vol, true);

        return true;
    }//Execute
}//CPHInline