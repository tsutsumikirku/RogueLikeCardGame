using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerExample : MonoBehaviour
{
    void Start()
    {
        // EventTrigger コンポーネントを取得または追加
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }

        // PointerClick イベント用のエントリを作成
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };

        // イベント時に実行するコールバックを追加
        entry.callback.AddListener((data) => { OnClick((PointerEventData)data); });

        // Entry を EventTrigger に追加
        trigger.triggers.Add(entry);
    }

    // クリックされたときの処理
    void OnClick(PointerEventData eventData)
    {
        // イベントが発生したオブジェクトを取得
        GameObject clickedObject = eventData.pointerPress;

        Debug.Log("Clicked Object: " + clickedObject.name);
    }
}