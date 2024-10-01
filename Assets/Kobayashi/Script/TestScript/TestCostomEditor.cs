using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardBase))]
public class TestCostomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ���̃C���X�y�N�^�[��\��
        base.OnInspectorGUI();

        // ���݂̑ΏۃI�u�W�F�N�g���擾
        CardBase card = (CardBase)target;

        // inputString������̏����𖞂����ꍇ�A�l��u��������
        int state = 1;
        int color = 1;
        foreach (var effect in card._cardEffect)
        {
            switch (effect)
            {
                case BuffCard:
                    if (card.cardData._infometion.Contains("state" + state))
                    {
                        Debug.Log("�����񂪒u���������܂����B");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"state{state}", effect.GetEffectClass<BuffCard>()._stats.ToString());
                        state++;
                    }
                    if (card.cardData._infometion.Contains("buffColor" + color))
                    {
                        Debug.Log("�����񂪒u���������܂����B");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"buffColor{color}", effect.GetEffectClass<BuffCard>()._buff.ToString());
                        color++;
                    }
                    break;
                case OneTimeBuffCard:
                    if (card.cardData._infometion.Contains("state" + state))
                    {
                        Debug.Log("�����񂪒u���������܂����B");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"state{state}", effect.GetEffectClass<OneTimeBuffCard>()._stats.ToString());
                        state++;
                    }
                    if (card.cardData._infometion.Contains("buffColor" + color))
                    {
                        Debug.Log("�����񂪒u���������܂����B");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"buffColor{color}", effect.GetEffectClass<OneTimeBuffCard>()._buff.ToString());
                        color++;
                    }
                    break;
            }
        }
        EditorUtility.SetDirty(target);
    }
    //�ύX�𔽉f������
}


