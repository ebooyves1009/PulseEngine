using PulseEngine;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableTest", menuName = "Asset/SR test", order = 1)]
public class TestAsset : ScriptableObject
{
    public CommandSequence SQ;
}