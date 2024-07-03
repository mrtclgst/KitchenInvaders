using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        GameScene,
        LoadingScene,
        MainMenuScene,
        LobbyScene,
        CharacterSelectScene,
    }

    private static Scene _targetScene;

    public static void LoadScene(Scene targetScene)
    {
        _targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetworkScene(Scene targetScene)
    {
        _targetScene = targetScene;
        NetworkManager.Singleton.SceneManager.LoadScene
            (targetScene.ToString(), LoadSceneMode.Single);
    }

    internal static void LoaderCallback()
    {
        SceneManager.LoadScene(_targetScene.ToString());
    }
}