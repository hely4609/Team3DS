using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WaitForFunction : CustomYieldInstruction
{
    bool isWaiting;
    public override bool keepWaiting => isWaiting;

    public WaitForFunction(System.Action wantFunction)
    {
        isWaiting = true;

        Run(wantFunction);
    }

    async void Run(System.Action wantfuction)
    {
        await Task.Run(wantfuction);

        isWaiting = false;
    }

}
