using System;
using Cysharp.Threading.Tasks;

public interface ILoadScream
{
    UniTaskVoid Open(Action a);
    UniTaskVoid Close(Action a);
}