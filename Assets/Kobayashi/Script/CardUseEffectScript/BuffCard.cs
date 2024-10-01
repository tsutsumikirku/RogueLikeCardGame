using UnityEngine;
public class BuffCard : IUseEffect
{
    public Buff _buff;
    [SerializeField] public int _stats;
    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        Debug.Log("ƒoƒt‚Ì’Ç‰Á");
        useCharacter._buff[_buff]+=_stats;
    }

    public T GetEffectClass<T>() where T : IUseEffect
    {
        if (this is T effect)
        {
            return effect;
        }
        return default;
    }
}
public class doubleAttack //: IUseEffect
{
    public void Effect(CharacterBase useCharacter, CharacterBase target)
    {
        throw new System.NotImplementedException();
    }
}
