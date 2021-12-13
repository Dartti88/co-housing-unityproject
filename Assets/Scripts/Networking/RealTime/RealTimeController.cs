using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;


[Serializable]
public class ProfileStatus
{
    public int profileID;
    public int loginStatus;
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class ProfileStatusesContainer { public ProfileStatus[] statuses; }

public class RealTimeController : MonoBehaviour
{
    public GameObject networkPlayerPrefab;
    
    // Contains all other logged in players excluding the local user
    public Dictionary<int, GameObject> otherPlayers = new Dictionary<int, GameObject>();
    
    float cooldown_updateDestinations;
    const float maxCooldown_updateDestinations = 2.0f;

    float cooldown_sendDestination;
    const float maxCooldown_sendDestination = 2.0f;

    float cooldown_fetchStatuses = 0.0f;
    const float maxCooldown_fetchStatuses = 3.0f;


    bool othersSpawned = false;

    ProfileHandler profileHandler;
    [HideInInspector] public GameObject localPlayer;
    NavMeshAgent localNavMeshAgent;
    
    private void Start()
    {
        profileHandler = FindObjectOfType<ProfileHandler>();
        localPlayer = PlayerController.Instance.gameObject;
        localNavMeshAgent = localPlayer.GetComponent<NavMeshAgent>();
        localNavMeshAgent.Warp(Client.Instance.initLocalPlayerPos); // Need to warp that fucker here.. ..that fucking time wasting problem, in the "local player" spawning
        Rotator.Instance.transform.position = Client.Instance.initLocalPlayerPos;
    }

    // Destroy all necessary stuff on destroy..
    private void OnDestroy()
    {
        foreach (KeyValuePair<int, GameObject> player in otherPlayers)
        {
            GameObject obj = player.Value;
            if (obj != null) GameObject.Destroy(obj);
        }

        otherPlayers.Clear();
    }

    private void Update()
    {
        if (cooldown_fetchStatuses <= 0.0f)
        {
            Client.Instance.BeginRequest_GetProfileStatuses(OnCompletion_FetchPlayerStatuses);
            Debug.Log("Player spawning triggered...");
            cooldown_fetchStatuses = maxCooldown_fetchStatuses;
        }
        else
        {
            cooldown_fetchStatuses -= 1.0f * Time.deltaTime;
        }
    }

    // Triggers a "visual emote" for a local (other)player object
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

    void OnCompletion_FetchPlayerStatuses(string serverResponse)
    {
        string json = "{\"statuses\": " + serverResponse + "}";
        try
        {
            ProfileStatusesContainer newStatusesContainer = JsonUtility.FromJson<ProfileStatusesContainer>(json);
            // Do we have these players or not? (spawn or despawn or? or..?)
            List<KeyValuePair<int, Vector3>> playersToSpawn = new List<KeyValuePair<int, Vector3>>();
            List<int> playersToDespawn = new List<int>();
            
            // Spawning
            foreach (ProfileStatus pStatus in newStatusesContainer.statuses)
            {
                // *Skip the current local player completely!
                if (pStatus.profileID == profileHandler.userProfile.profileID)
                    continue;

                bool newPlayerFound = true;// *by default here, we assume that we ARE going to find a new player to spawn
                // Go through each player from the newly fetched array
                foreach (KeyValuePair<int, GameObject> existingPlayer in otherPlayers)
                {
                    // If this player already exists on the server side -> block adding it to the "spawning list" and update its' local shit...
                    if (pStatus.profileID == existingPlayer.Key)
                    {
                        newPlayerFound = false;
                        UpdatePlayer(existingPlayer, pStatus);
                        break;
                    }
                }
                if (newPlayerFound) playersToSpawn.Add(new KeyValuePair<int, Vector3>(pStatus.profileID, new Vector3(pStatus.x, pStatus.y, pStatus.z)));
            }

            // Despawning
            foreach (KeyValuePair<int, GameObject> existingPlayer in otherPlayers)
            {
                // If this player doesn't exist on server side anymore, but it does locally -> destroy it
                // *We have to search for match again for each "server side player", we just fetched..
                bool deletedPlayerFound = true; // Again we assume here that we DID find a "deleted/logged out" player...
                foreach (ProfileStatus pStatus2 in newStatusesContainer.statuses)
                {
                    // If this player still does exists -> block its' despawning
                    if (pStatus2.profileID == existingPlayer.Key)
                    {
                        deletedPlayerFound = false;
                        break;
                    }
                }
                // If we got here, its safe to assume, we can despawn this player..
                if (deletedPlayerFound) playersToDespawn.Add(existingPlayer.Key);
            }

            // First DEspawn all logged out players...
            foreach (int pID in playersToDespawn) DeSpawnPlayer(pID);
            
            // And finally spawn all the newly logged in players...
            foreach (KeyValuePair<int, Vector3> p in playersToSpawn) SpawnPlayer(p.Key, p.Value);
        }
        catch (Exception e)
        {
            Debug.Log("No profile statuses were found");
        }
    }

    // Updates local players, depending on what our database earlier told us, their statuses were?
    // *Touches only local stuff!
    void UpdatePlayer(KeyValuePair<int, GameObject> playerToUpdate, ProfileStatus status)
    {
        if (playerToUpdate.Value == null)
        {
            Debug.Log("ERROR! Attempted to update non spawned player!");
            return;
        }

        // Set player's destination to match server side
        playerToUpdate.Value.GetComponent<CharacterController>().SetGoal(new Vector3(status.x, status.y, status.z));
    }

    // Spawns a new player object
    void SpawnPlayer(int profileID, Vector3 pos)
    {
        // Attempt to find match from current profile list...
        Profile playerProfile = Array.Find(Client.Instance.profile_list.profiles, e => e.profileID == profileID);
        if (playerProfile == null)
        {
            // -> If not found?
            //      -> Skip spawning this player until the next interval, and force updating the current profile list from the server (new user has reqistered...?)
            //      (*BUT only if this request wasn't already in progress, in case of some unknown fuckery..)
            //if (!Client.Instance.RequestInProgress[Client.RequestName.GetAllProfiles])
            //    Client.Instance.BeginRequest_GetAllProfiles(null);

            Client.Instance.BeginRequest_GetAllProfiles( // <- Actually it would be fucking beautiful, if this worked??!?!
                response =>
                {
                    SpawnPlayer(profileID, pos);
                }
            );
            return;
        }

        GameObject spawnedPlayer = GameObject.Instantiate(networkPlayerPrefab, pos, Quaternion.identity, Client.Instance.transform);
        spawnedPlayer.GetComponent<CharacterController>().Init(playerProfile.avatarID);
        otherPlayers.Add(profileID, spawnedPlayer);
        Debug.Log("New player spawned!");
    }

    // Destroys a player object and all "real time controller stuff" related to it..
    void DeSpawnPlayer(int profileID)
    {
        GameObject p = otherPlayers[profileID];
        if (p != null)
        {
            GameObject.Destroy(p);
            otherPlayers.Remove(profileID);
        }
        else
        {
            Debug.Log("ERROR! Attempted to DEspawn non existing profileID!");
        }
    }

    
}
