using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentStatsUI : MonoBehaviour
{
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI bestReward;
    public TextMeshProUGUI actionText;
    public Transform arrow;
    float bestRewardValue;
    public RawImage mlVision;
    public Dictionary<int, string> namedActions = new Dictionary<int, string> {
        { 0, "Nothing" },
        { 1, "Right" },
        { 2, "Left" },
        { 3, "Forward" },
        { 4, "Back" },
        { -1, "" }
    };
    public void SetText(int action, float reward)
    {
        rewardText.text = "Current Reward: " + reward.ToString();
        actionText.text = "Current Action: " + namedActions[action];
    }

    public void SetInfo(Vector3 action, float reward)
    {
        SetText(-1, reward);
        arrow.forward = action;

    }

    public void SetBestReward(float reward)
    {
        bestRewardValue = Mathf.Max(bestRewardValue, reward);

        bestReward.text = "Best Reward: " + bestRewardValue.ToString();
    }

    public void SetRenderTexture(RenderTexture rt)
    {
        mlVision.texture = rt;
    }
}
