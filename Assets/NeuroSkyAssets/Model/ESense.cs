

[System.Serializable]
public class ESense
{
    public int attention;
    public int meditation;
}

[System.Serializable]
public class EegPower
{
    public float delta;
    public float theta;
    public float lowAlpha;
    public float highAlpha;
    public float lowBeta;
    public float highBeta;
    public float lowGamma;
    public float highGamma;
}

[System.Serializable]
public class BrainWaveData
{
    public ESense eSense;
    public EegPower eegPower;
    public int poorSignalLevel;
}