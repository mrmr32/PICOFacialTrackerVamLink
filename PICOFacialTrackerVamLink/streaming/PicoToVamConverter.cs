using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICOFacialTrackerVamLink;

public sealed class PicoToVamConverter : VAMConverter<BlendShape>
{
    public PicoToVamConverter(DataProvider<BlendShape> provider) : base(provider)
    {
    }

    public override IDictionary<string, float> GetData()
    {
        Dictionary<string,float> convertedData = new Dictionary<string,float>();
        IDictionary<BlendShape, float> originalData = this.provider.GetData();

        UpdateExpression(ref convertedData, originalData);

        return convertedData;
    }

    // @see https://hub.vive.com/storage/docs/en-us/UnityXR/UnityXRLipExpression.html
    private static void UpdateExpression(ref Dictionary<string, float> convertedData, IDictionary<BlendShape, float> originalData)
    {
        #region Jaw
        convertedData["Jaw_Right"] = originalData[BlendShape.JawRight];
        convertedData["Jaw_Left"] = originalData[BlendShape.JawLeft];
        convertedData["Jaw_Forward"] = originalData[BlendShape.JawForward];
        convertedData["Jaw_Open"] = originalData[BlendShape.JawOpen];
        #endregion
        #region Cheek
        convertedData["Cheek_Puff_Right"] = convertedData["Cheek_Puff_Left"] = originalData[BlendShape.CheekPuff];
        convertedData["Cheek_Suck"] = 0f;
        #endregion
        #region Mouth
        convertedData["Mouth_Ape_Shape"] = 0f;
        convertedData["Mouth_Upper_Right"] = convertedData["Mouth_Lower_Right"] = originalData[BlendShape.MouthRight];
        convertedData["Mouth_Upper_Left"] = convertedData["Mouth_Lower_Left"] = originalData[BlendShape.MouthLeft];
        convertedData["Mouth_Upper_Overturn"] = convertedData["Mouth_Lower_Overturn"] = originalData[BlendShape.MouthFunnel];
        convertedData["Mouth_Pout"] = originalData[BlendShape.MouthPucker];
        convertedData["Mouth_Smile_Right"] = originalData[BlendShape.MouthSmile_R];
        convertedData["Mouth_Smile_Left"] = originalData[BlendShape.MouthSmile_L];
        convertedData["Mouth_Sad_Right"] = originalData[BlendShape.MouthFrown_R];
        convertedData["Mouth_Sad_Left"] = originalData[BlendShape.MouthFrown_L];
        convertedData["Mouth_Upper_UpRight"] = originalData[BlendShape.MouthUpperUp_R];
        convertedData["Mouth_Upper_UpLeft"] = originalData[BlendShape.MouthUpperUp_L];
        convertedData["Mouth_Lower_DownRight"] = originalData[BlendShape.MouthLowerDown_R];
        convertedData["Mouth_Lower_DownLeft"] = originalData[BlendShape.MouthLowerDown_L];
        convertedData["Mouth_Upper_Inside"] = convertedData["Mouth_Lower_Inside"] = originalData[BlendShape.MouthRollUpper];
        convertedData["Mouth_Lower_Overlay"] = 0f;
        #endregion
        #region Tongue
        convertedData["Tongue_LongStep1"] = 0f;
        convertedData["Tongue_Up"] = 0f;
        convertedData["Tongue_Left"] = 0f;
        convertedData["Tongue_Right"] = 0f;
        convertedData["Tongue_Down"] = 0f;
        convertedData["Tongue_Roll"] = 0f;
        convertedData["Tongue_LongStep2"] = originalData[BlendShape.TongueOut];
        convertedData["Tongue_UpRight_Morph"] = 0f;
        convertedData["Tongue_UpLeft_Morph"] = 0f;
        convertedData["Tongue_DownRight_Morph"] = 0f;
        convertedData["Tongue_DownLeft_Morph"] = 0f;
        #endregion
    }
}
