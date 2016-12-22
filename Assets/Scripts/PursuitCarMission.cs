﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PursuitCarMission :  MissionsAbstract {

    public List<MissionTargets> MissionTargets; //Mission target
    MissionManager MM; //Mission Manager
    AudioSource CollectSound; //Sound that plays when mission's objective is completed 

    int TargetIndex = 0;

    public override void InitiateMission(MissionManager missionManager, int targetIndex = 0) {
        Debug.Log("Initiate Pursuit Car Mission");
        TargetIndex = targetIndex;

        MM = missionManager;
        MM.TargetScript.SetMission(this); //Set mission on Targets script 
        MM.TargetScript.GetComponent<Targets>().NewTarget(MissionTargets[targetIndex].Target); //Set target in Targets script
        CollectSound = MissionTargets[targetIndex].Target.GetComponent<AudioSource>();
    }

    public override void EndMission() {
        Debug.Log("End Pursuit Car Mission");
        CollectSound.Play();
        MM.EndCurrentMission();
    }

    public override string GetDisplayText() {
        return MissionTargets[TargetIndex].MissionDescription;
    }
}
