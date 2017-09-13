using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
	public class TotalSettlementInfo : MonoBehaviour
	{
	    public GameObject championIcon;
	    public Image avatar;
	    public Text userName;
	
	    public GameObject firstTrophy;
	    public GameObject secondTrophy;
	    public GameObject thirdTrophy;
	
	    public SpriteNumber totalScorePositive;
	    public SpriteNumber totalScoreNagative;
	    public SpriteNumber roomCost;
	
	    public void SetSettlementInfo(PlayerSettlement p)
	    {
	        PlayerMgr.Inst.GetPlayer(p.id).SetAvatar(this.avatar);
	        this.userName.text = p.userName;
	        switch (p.totalRank)
	        {
	            case RankType.None:
	                break;
	            case RankType.Up:
	                championIcon.SetActive(true);
	                firstTrophy.SetActive(true);
	                break;
	            case RankType.Middle:
	                secondTrophy.SetActive(true);
	                break;
	            case RankType.Down:
	                thirdTrophy.SetActive(true);
	                break;
	            default:
	                break;
	        }
	        if (p.totalScore >= 0)
	        {
	            totalScorePositive.Number = "+" + p.totalScore;
	        }
	        else
	        {
	            totalScoreNagative.Number = "-" + Mathf.Abs(p.totalScore);
	        }
	        roomCost.Number = "-" + Mathf.Abs(p.roomCost).ToString();
	    }
	}
}