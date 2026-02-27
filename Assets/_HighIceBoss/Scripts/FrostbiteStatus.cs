using UnityEngine;

public class FrostbiteStatus : MonoBehaviour
{
    public int stacks;
    public int maxStacks = 7;

    public float slowPerStack = 0.08f;
    public float stackDuration = 6f;

    float[] expiry;

    private void Awake()
    {
        expiry = new float[maxStacks];
    }

    private void Update()
    {
        float t = Time.time;
        int newStack = 0;
        for (int i = 0; i < stacks; ++i)
        {
            if (expiry[i] > t)
            {
                newStack++;
            }
        }

        stacks = newStack;
    }

    public void AddStack()
    {
        if (stacks >= maxStacks) { return; }

        expiry[stacks] = Time.time + stackDuration;
        stacks++;

        if (stacks >= maxStacks) 
        { 
            //GetComponent<PlayerMovement>().Freeze(1.5f); 
        }
    }

    public float GetMoveMultiplier()
    {
        float slow = stacks * slowPerStack;
        return Mathf.Clamp01(1f - slow);
    }

    public void ClearAll() => stacks = 0;
}
