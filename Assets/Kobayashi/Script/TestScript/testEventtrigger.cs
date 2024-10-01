using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerExample : MonoBehaviour
{
    void Start()
    {
        // EventTrigger �R���|�[�l���g���擾�܂��͒ǉ�
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }

        // PointerClick �C�x���g�p�̃G���g�����쐬
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };

        // �C�x���g���Ɏ��s����R�[���o�b�N��ǉ�
        entry.callback.AddListener((data) => { OnClick((PointerEventData)data); });

        // Entry �� EventTrigger �ɒǉ�
        trigger.triggers.Add(entry);
    }

    // �N���b�N���ꂽ�Ƃ��̏���
    void OnClick(PointerEventData eventData)
    {
        // �C�x���g�����������I�u�W�F�N�g���擾
        GameObject clickedObject = eventData.pointerPress;

        Debug.Log("Clicked Object: " + clickedObject.name);
    }
}