//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using TNet;

/// <summary>
/// This script shows how to destroy an object when clicked on.
/// </summary>

public class ExampleDestroy : TNBehaviour
{
    void OnClick() { DestroySelf(); }
}
