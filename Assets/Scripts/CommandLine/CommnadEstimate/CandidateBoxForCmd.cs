using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandidateBoxForCmd : CandidateBox
{
    [SerializeField] private Text _selectedTriangle;

    protected override void SetActive()
    {
        _selectedTriangle.gameObject.SetActive(true);
    }

    protected override void CancellActive()
    {
        _selectedTriangle.gameObject.SetActive(false);
    }
}