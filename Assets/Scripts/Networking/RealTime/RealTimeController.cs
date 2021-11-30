using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

[Serializable]
public class CharacterDestination
{
    public int profileID;
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class CharacterDestinationsContainer { public CharacterDestination[] destinations; }

public class RealTimeController : MonoBehaviour
{
    public GameObject networkPlayerPrefab;
    public CharacterDestinationsContainer destinationsContainer = new CharacterDestinationsContainer();

    // Contains all other logged in players excluding the local user
    public Dictionary<int, GameObject> otherPlayers;
    
    float cooldown_updateDestinations;
    const float maxCooldown_updateDestinations = 2.0f;

    float cooldown_sendDestination;
    const float maxCooldown_sendDestination = 2.0f;

    bool othersSpawned = false;

    ProfileHandler profileHandler;
    public GameObject localPlayer;
    NavMeshAgent localNavMeshAgent;

    private void Start()
    {
        profileHandler = FindObjectOfType<ProfileHandler>();
        localNavMeshAgent = localPlayer.GetComponent<NavMeshAgent>();
        BeginSpawnOtherPlayers();
    }

    private void Update()
    {
        UpdateCharacterDestinations();
    }

    // Updates every local character's pathfinding destination from the server
    // *DOES NOT DO ANY WEB REQUESTS!
    public void UpdateCharacterDestinations()
    {
        if (othersSpawned)
        {
            if (cooldown_updateDestinations > 0.0f)
            {
                cooldown_updateDestinations -= 1.0f * Time.deltaTime;
            }
            else
            {
                Client.Instance.BeginRequest_GetCharacterDestinations(UpdateOthersDestinations);
                Debug.Log("Real time update triggered(GetDestinations)");
                cooldown_updateDestinations = maxCooldown_updateDestinations;
            }
        }
    }

    // Sends local user's character's pathfinding destination to the server
    // *DOES NOT TOUCH ANY LOCAL SHIT!
    /*public void SendCharacterDestination()
    {
        if (cooldown_sendDestination > 0.0f)
        {
            cooldown_sendDestination -= 1.0f * Time.deltaTime;
        }
        else
        {
            Client.Instance.BeginRequest_SendCharacterDestination(profileHandler.userProfile.profileID, localNavMeshAgent.destination, null);
            Debug.Log("Real time update triggered(SendDestination)");
            cooldown_sendDestination = maxCooldown_sendDestination;
        }
    }*/

    public void BeginSpawnOtherPlayers()
    {
        Client.Instance.BeginRequest_GetCharacterDestinations(OnCompletion_InitAllPlayers);
    }

    // Spawns all other players depending on whos in the "CharacterDestinations" - table
    void OnCompletion_InitAllPlayers(string response)
    {
        UpdateDestinations(response);
        otherPlayers = new Dictionary<int, GameObject>();

        foreach (CharacterDestination cd in destinationsContainer.destinations)
        {
            // Skip local player's profile
            if (cd.profileID == profileHandler.userProfile.profileID)
                continue;

            // first find profile of this id
            foreach (Profile p in Client.Instance.profile_list.profiles)
            {
                if (cd.profileID == p.profileID)
                {
                    GameObject spawnedPlayer = GameObject.Instantiate(networkPlayerPrefab, Client.Instance.transform);
                    spawnedPlayer.GetComponent<CharacterController>().Init(p.avatarID);
                    otherPlayers.Add(cd.profileID, spawnedPlayer);
                    break;
                }
            }
        }

        if (otherPlayers.Count > 0)
            othersSpawned = true;

        Debug.Log("Other players spawned, Local user id = " + profileHandler.userProfile.profileID);
    }

    void UpdateOthersDestinations(string serverResponse)
    {
        UpdateDestinations(serverResponse);
        foreach (CharacterDestination cd in destinationsContainer.destinations)
        {
            // Skip local player's profile
            if (cd.profileID == profileHandler.userProfile.profileID)
                continue;

            if (otherPlayers.ContainsKey(cd.profileID))
                otherPlayers[cd.profileID].GetComponent<CharacterController>().SetGoal(new Vector3(cd.x, cd.y, cd.z));
            else
                Debug.Log("WARNING! @UpdateOthersDestinations(string) : Tried to access invalid key in 'otherPlayers' - dictionary");
        }
    }

    void UpdateDestinations(string response)
    {
        string json = "{\"destinations\": " + response + "}";
        try
        {
            destinationsContainer = JsonUtility.FromJson<CharacterDestinationsContainer>(json);
        }
        catch (Exception e)
        {
            destinationsContainer = new CharacterDestinationsContainer(); // init to empty list if not found
            Debug.Log("No available character destinations found!");
        }
    }

    // JUST FOR TESTING EMOTE TRIGGERING
    public void TriggerEmote(int profileID, int emoteID)
    {
        GameObject obj = otherPlayers[profileID].transform.Find("Emotecontainer").transform.Find("EmotePicture").gameObject;
        if (obj)
        {
            obj.SetActive(true);
            EmoteBillboard component = obj.GetComponent<EmoteBillboard>();
            component.UseEmote(emoteID);
        }
    }
}
