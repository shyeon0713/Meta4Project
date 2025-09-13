using System.Collections;
using System.Security.Cryptography;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Networking;



public class DayCheck : MonoBehaviour
{
    [SerializeField]
    private int currentDay;
    public int CurrentDay => currentDay;

    private const string URL = "http://127.0.0.1:8000/save/";


    #region Day가 끝날 경우 다음날로 넘어가기 전에 신호보내기
    public IEnumerator LogDayCheck(int dayNumber, int talkchance)
    {
        //추후에 URL만 모아둔 헤더파일 추가하기
        using (UnityWebRequest request = UnityWebRequest.Get(URL))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {// 연결이 제대로 되지 않은 경우
                Debug.LogError($"[DayCheck] DB 로드 실패 → {request.error}");
                yield break;
            }

            string json = request.downloadHandler.text; //변수값만 받기때문에

            Savedata data = JsonUtility.FromJson<Savedata>(json); //Json구조와 같이 획일화

            if (talkchance >= 5 && data.day == dayNumber)
            {
                currentDay = dayNumber + 1;
                DayChanged?.Invoke(currentDay); //Day가 바뀐것을 외부에 알림
            }

        }
    }
        public System.Action<int> DayChanged;

}
#endregion
