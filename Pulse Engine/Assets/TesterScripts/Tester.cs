using PulseEngine.Datas;
using PulseEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PulseEngine.Modules.Localisator;
using PulseEngine.Modules.CharacterCreator;
using System.Threading.Tasks;
using PulseEngine.Modules.Commander;

public class Tester : MonoBehaviour
{
    public CommandsLibrary source;
    private CommandsLibrary asset;
    public bool playingCmd;
    public int possiblesPaths = 0;
    private CommandSequence currentSequence;
    private Command currentCmd;
    private Dictionary<DataLocation, CommandPath> resumeList = new Dictionary<DataLocation, CommandPath>();

    private void OnGUI()
    {
        if(asset == null && source != null)
        {
            asset = Core.LibraryClone(source);
        }
        if (asset != null) {
            if (currentSequence == null)
            {
                GUILayout.Button("Choose Command list");
                for (int i = 0; i < asset.DataList.Count; i++)
                {
                    int k = i;
                    if (GUILayout.Button("List " + k))
                    {
                        currentSequence = (CommandSequence)asset.DataList[k];
                        if (resumeList.ContainsKey(currentSequence.Location))
                        {
                            int index = currentSequence.Sequence.FindIndex(cmd => { return cmd.Path == resumeList[currentSequence.Location]; });
                            if (currentSequence.Sequence.IndexInCollection(index))
                                currentCmd = currentSequence.Sequence[index];
                            else
                            {
                                resumeList.Remove(currentSequence.Location);
                                currentCmd = currentSequence.Sequence[0];
                            }
                        }
                        else
                        {
                            currentCmd = currentSequence.Sequence[0];
                        }
                        ExecuteCommand(currentCmd);
                        break;
                    }
                }
            }
            else
            {
                if (playingCmd)
                {
                    GUILayout.Button("Playing CMD: " + currentCmd.Path.Label);
                    GUILayout.Button("Choosen Command list: " + currentSequence.Label);
                }
                else
                {
                    int index = currentSequence.Sequence.FindIndex(cmd => { return cmd == currentCmd; });
                    if (possiblesPaths <= 0)
                    {
                        currentSequence = null;
                        currentCmd = Command.NullCmd;
                    }
                    else if(currentSequence.Sequence.IndexInCollection(index))
                    {
                        for(int i = 0; i < currentSequence.Sequence[index].Outputs.Count; i++)
                        {
                            int k = i;
                            if (GUILayout.Button("Go for path "+k))
                            {
                                int pathIndex = currentSequence.Sequence.FindIndex(cmd => { return cmd.Path == currentSequence.Sequence[index].Outputs[k]; });
                                if (currentSequence.Sequence.IndexInCollection(pathIndex))
                                {
                                    currentCmd = currentSequence.Sequence[pathIndex];
                                    ExecuteCommand(currentCmd);
                                }
                                else
                                {
                                    pathIndex = currentSequence.specialCmds.FindIndex(cmd => { return cmd.Path == currentSequence.Sequence[index].Outputs[k]; });
                                    if (currentSequence.specialCmds.IndexInCollection(pathIndex) && currentSequence.Sequence[index].Outputs[k] == CommandPath.BreakPath)
                                    {
                                        if (resumeList.ContainsKey(currentSequence.Location))
                                            resumeList[currentSequence.Location] = currentCmd.Path;
                                        else
                                            resumeList.Add(currentSequence.Location, currentCmd.Path);
                                    }
                                    else if (resumeList.ContainsKey(currentSequence.Location))
                                        resumeList.Remove(currentSequence.Location);
                                    currentSequence = null;
                                    currentCmd = Command.NullCmd;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public async Task<Command> ExecuteCommand(Command cmd)
    {
        possiblesPaths = cmd.Outputs.Count;
        playingCmd = true;
        cmd.State = CommandState.playing;
        await Task.Delay(5000);
        cmd.State = CommandState.done;
        playingCmd = false;
        return cmd;
    }

    Color[] palette = new Color[]
    {
        Color.green,
        Color.red,
        Color.yellow,
        Color.blue,
        Color.cyan,
        Color.black,
    };

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(representation[0].position, Vector3.one * 0.5f);
        //for (int i = 1; i < representation.Count; i++)
        //{
        //    var color = palette[i % palette.Length];
        //    Gizmos.color = color;
        //    Gizmos.DrawCube(representation[i].position, Vector3.one * 0.5f);
        //    Gizmos.color = color;
        //    Gizmos.DrawLine(representation[i].position, representation[i].parentPos);
        //}
    }


    private void FindRecursive(Command c)
    {
        //for(int i = 0; i < lenght; i++)
        //{
        //    var ch = c.GetChild(i);
        //    Debug.Log("found child at: " + ch.NodePosition + " parented to: " + ch.Parent.NodePosition);
        //    representation.Add((ch.NodePosition, ch.Parent.NodePosition));
        //    FindRecursive(ch);
        //}
    }


    private async void Spawn()
    {
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * 5;
        Vector3 position = new Vector3(randPos.x, 0, randPos.y);
        //await CharacterCreator.SpawnCharacter(new DataLocation { id = spawnCount, globalLocation = 0, localLocation = 0 }, position);
    }

    private async void LocData()
    {
        string t = await Localisator.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.title, true);
        string d = await Localisator.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.description, true);
        //text = t.Italic() + " \n " + d;
        //string w = await CoreData.ManagerAsyncMethod<string>(ModulesManagers.Localisator, "TextData", new object[] { (object)new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }
        //    , (object)DatalocationField.title,
        //    (object)false });
        //Debug.Log("C");
        //text = w;
    }

    private static dynamic PlayGround()
    {
        return 1 + 1;
    }
}



[Serializable]
public struct Serial
{
    public int ID;
    public List<string> childrens;

    public Serial(int _id)
    {
        ID = _id;
        childrens = new List<string>();
    }
}






