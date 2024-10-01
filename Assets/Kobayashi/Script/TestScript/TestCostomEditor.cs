using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardBase))]
public class TestCostomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 元のインスペクターを表示
        base.OnInspectorGUI();

        // 現在の対象オブジェクトを取得
        CardBase card = (CardBase)target;

        // inputStringが特定の条件を満たす場合、値を置き換える
        int state = 1;
        int color = 1;
        foreach (var effect in card._cardEffect)
        {
            switch (effect)
            {
                case BuffCard:
                    if (card.cardData._infometion.Contains("state" + state))
                    {
                        Debug.Log("文字列が置き換えられました。");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"state{state}", effect.GetEffectClass<BuffCard>()._stats.ToString());
                        state++;
                    }
                    if (card.cardData._infometion.Contains("buffColor" + color))
                    {
                        Debug.Log("文字列が置き換えられました。");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"buffColor{color}", effect.GetEffectClass<BuffCard>()._buff.ToString());
                        color++;
                    }
                    break;
                case OneTimeBuffCard:
                    if (card.cardData._infometion.Contains("state" + state))
                    {
                        Debug.Log("文字列が置き換えられました。");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"state{state}", effect.GetEffectClass<OneTimeBuffCard>()._stats.ToString());
                        state++;
                    }
                    if (card.cardData._infometion.Contains("buffColor" + color))
                    {
                        Debug.Log("文字列が置き換えられました。");
                        card.cardData._infometion = card.cardData._infometion.Replace(
                            $"buffColor{color}", effect.GetEffectClass<OneTimeBuffCard>()._buff.ToString());
                        color++;
                    }
                    break;
            }
        }
        EditorUtility.SetDirty(target);
    }
    //変更を反映させる
}


