﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace r.e.p.o_cheat
{
    public static class UIHelper
    {
        private static float x, y, width, height, margin, controlHeight, controlDist, nextControlY;
        private static int columns = 1;
        private static int currentColumn = 0;
        private static int currentRow = 0;

        private static float debugX, debugY, debugWidth, debugHeight, debugMargin, debugControlHeight, debugControlDist, debugNextControlY;
        private static int debugCurrentColumn = 0;
        private static int debugCurrentRow = 0;
        private static int debugColumns = 1;

        private static GUIStyle debugLabelStyle = null;

        public static bool ButtonBool(string text, bool value, float? customX = null, float? customY = null)
        {
            Rect rect = NextControlRect(customX, customY);
            string displayText = $"{text} {(value ? "✔" : " ")}";
            GUIStyle style = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, normal = { textColor = value ? Color.green : Color.red } };
            return GUI.Button(rect, displayText, style) ? !value : value;
        }

        public static bool Checkbox(string text, bool value, float? customX = null, float? customY = null)
        {
            return GUI.Toggle(NextControlRect(customX, customY), value, text);
        }

        public static void Begin(string text, float _x, float _y, float _width, float _height, float InstructionHeight, float _controlHeight, float _controlDist)
        {
            x = _x; y = _y; width = _width; height = _height; margin = InstructionHeight; controlHeight = _controlHeight; controlDist = _controlDist;
            nextControlY = y + margin + 60;
            GUI.Box(new Rect(x, y, width, height), text);
            ResetGrid();
        }

        public static void BeginDebugMenu(string text, float _x, float _y, float _width, float _height, float _margin, float _controlHeight, float _controlDist)
        {
            debugX = _x; debugY = _y; debugWidth = _width; debugHeight = _height; debugMargin = _margin; debugControlHeight = _controlHeight; debugControlDist = _controlDist;
            debugNextControlY = debugY + debugMargin + 30;
            GUI.Box(new Rect(debugX, debugY, debugWidth, debugHeight), text);
            if (debugLabelStyle == null)
            {
                debugLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    wordWrap = true,
                    clipping = TextClipping.Clip,
                    fontSize = 12,
                    padding = new RectOffset(2, 2, 2, 2)
                };
            }
        }

        private static Rect NextControlRect(float? customX = null, float? customY = null)
        {
            float controlX = customX ?? (x + margin + currentColumn * ((width - (columns + 1) * margin) / columns));
            float controlY = customY ?? nextControlY;
            float controlWidth = customX == null ? ((width - (columns + 1) * margin) / columns) : width - 2 * margin;

            Rect rect = new Rect(controlX, controlY, controlWidth, controlHeight);

            if (customX == null && customY == null)
            {
                currentColumn++;
                if (currentColumn >= columns)
                {
                    currentColumn = 0;
                    currentRow++;
                    nextControlY += controlHeight + controlDist;
                }
            }

            return rect;
        }

        private static Rect NextDebugControlRect()
        {
            float controlX = debugX + debugMargin + debugCurrentColumn * (debugWidth / debugColumns);
            float controlY = debugNextControlY;
            Rect rect = new Rect(controlX, controlY, debugWidth - debugMargin * 2, debugControlHeight);
            debugCurrentColumn++;
            if (debugCurrentColumn >= debugColumns)
            {
                debugCurrentColumn = 0;
                debugCurrentRow++;
            }
            return rect;
        }

        public static bool Button(string text, float? customX = null, float? customY = null)
        {
            return GUI.Button(NextControlRect(customX, customY), text);
        }

        public static bool Button(string text, float customX, float customY, float width, float height)
        {
            Rect rect = new Rect(customX, customY, width, height);
            return GUI.Button(rect, text);
        }

        public static string MakeEnable(string text, bool state) => $"{text}{(state ? "ON" : "OFF")}";
        public static void Label(string text, float? customX = null, float? customY = null) => GUI.Label(NextControlRect(customX, customY), text);
        public static float Slider(float val, float min, float max, float? customX = null, float? customY = null)
        {
            Rect rect = NextControlRect(customX, customY);

            // Estilo personalizado para o slider
            GUIStyle sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
            {
                normal = { background = MakeSolidBackground(new Color(0.7f, 0.7f, 0.7f), 1f) },
                hover = { background = MakeSolidBackground(new Color(0.8f, 0.8f, 0.8f), 1f) },
                active = { background = MakeSolidBackground(new Color(0.9f, 0.9f, 0.9f), 1f) } 
            };

            GUIStyle thumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb)
            {
                normal = { background = MakeSolidBackground(Color.white, 1f) },
                hover = { background = MakeSolidBackground(new Color(0.9f, 0.9f, 0.9f), 1f) },
                active = { background = MakeSolidBackground(Color.green, 1f) }
            };

            return Mathf.Round(GUI.HorizontalSlider(rect, val, min, max, sliderStyle, thumbStyle));
        }
        private static Texture2D MakeSolidBackground(Color color, float alpha)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(color.r, color.g, color.b, alpha));
            texture.Apply();
            return texture;
        }
        public static void DebugLabel(string text)
        {
            Rect rect = NextDebugControlRect();
            float textHeight = debugLabelStyle.CalcHeight(new GUIContent(text), rect.width);
            rect.height = Mathf.Max(textHeight, debugControlHeight);
            GUI.Label(rect, text, debugLabelStyle);
            debugNextControlY = rect.y + rect.height + 5;
        }

        public static void ResetGrid() { currentColumn = 0; currentRow = 0; nextControlY = y + margin + 60; }
        public static void ResetDebugGrid() { debugCurrentColumn = 0; debugCurrentRow = 0; debugNextControlY = debugY + debugMargin; }
    }

    public class Hax2 : MonoBehaviour
    {
        private float nextUpdateTime = 0f;
        private const float updateInterval = 10f;

        private int selectedPlayerIndex = 0;
        private List<string> playerNames = new List<string>();
        private List<object> playerList = new List<object>();
        private int selectedEnemyIndex = 0;
        private List<string> enemyNames = new List<string>();
        private List<Enemy> enemyList = new List<Enemy>();
        private float oldSliderValue = 0.5f;
        private float oldSliderValueStrength = 0.5f;
        private float sliderValue = 0.5f;
        public static float sliderValueStrength = 0.5f;
        public static float offsetESp = 0.5f;
        private bool showMenu = true;
        public static bool godModeActive = false;
        public static bool infiniteHealthActive = false;
        public static bool stamineState = false;
        public static List<DebugLogMessage> debugLogMessages = new List<DebugLogMessage>();
        private bool showDebugMenu = false;
        private Vector2 playerScrollPosition = Vector2.zero;
        private Vector2 enemyScrollPosition = Vector2.zero;

        private enum MenuCategory { Player, ESP, Combat, Misc, Enemies, Items }
        private MenuCategory currentCategory = MenuCategory.Player;

        public static float staminaRechargeDelay = 1f;
        public static float staminaRechargeRate = 1f;
        public static float oldStaminaRechargeDelay = 1f;
        public static float oldStaminaRechargeRate = 1f;

        public static float jumpForce = 1f;
        public static float customGravity = 1f;
        public static int extraJumps = 1;
        public static float flashlightIntensity = 1f;
        public static float crouchDelay = 1f;
        public static float crouchSpeed = 1f;

        public static float OldflashlightIntensity = 1f;
        public static float OldcrouchDelay = 1f;
        public static float OldjumpForce = 1f;
        public static float OldcustomGravity = 1f;
        public static float OldextraJumps = 1f;
        public static float OldcrouchSpeed = 1f;

        private List<ItemTeleport.GameItem> itemList = new List<ItemTeleport.GameItem>();
        private int selectedItemIndex = 0;
        private Vector2 itemScrollPosition = Vector2.zero;
        private float lastItemListUpdateTime = 0f;
        private const float itemListUpdateInterval = 2f;
        private bool isDragging = false;
        private Vector2 dragOffset;
        private float menuX = 50f;
        private float menuY = 50f;
        private const float titleBarHeight = 30f;

        private static bool cursorStateInitialized = false;

        public void Start()
        {
            UpdateCursorState();

            DebugCheats.texture2 = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            DebugCheats.texture2.SetPixels(new[] { Color.red, Color.red, Color.red, Color.red });
            DebugCheats.texture2.Apply();

            var playerHealthType = Type.GetType("PlayerHealth, Assembly-CSharp");
            if (playerHealthType != null)
            {
                Log1("playerHealthType não é null");
                Health_Player.playerHealthInstance = FindObjectOfType(playerHealthType);
                Log1(Health_Player.playerHealthInstance != null ? "playerHealthInstance não é null" : "playerHealthInstance null");
            }
            else Log1("playerHealthType null");

            var playerMaxHealth = Type.GetType("ItemUpgradePlayerHealth, Assembly-CSharp");
            if (playerMaxHealth != null)
            {
                Health_Player.playerMaxHealthInstance = FindObjectOfType(playerMaxHealth);
                Log1("playerMaxHealth não é null");
            }
            else Log1("playerMaxHealth null");
        }

        public void Update()
{
    Strength.UpdateStrength();

    // Limit update frequency to prevent lag
    if (Time.time >= nextUpdateTime)
    {
        DebugCheats.UpdateEnemyList();
        Log1("Lista de inimigos atualizada!");
        nextUpdateTime = Time.time + updateInterval;
    }

    // Reduce item list updates from every frame to every 5 seconds
    if (Time.time - lastItemListUpdateTime > 5f)  
    {
        UpdateItemList();
        itemList = ItemTeleport.GetItemList();
        lastItemListUpdateTime = Time.time;
    }

    if (oldSliderValue != sliderValue)
    {
        PlayerController.RemoveSpeed(sliderValue);
        oldSliderValue = sliderValue;
    }

    if (oldSliderValueStrength != sliderValueStrength)
    {
        Strength.MaxStrength();
        oldSliderValueStrength = sliderValueStrength;
    }

    if (playerColor.isRandomizing) 
    {
        playerColor.colorRandomizer();
    }

    // Prevent excessive logging by adding a cooldown
    if (Time.time - lastItemListUpdateTime > 10f)  
    {
        Log1($"Item list contains {itemList.Count} items.");
        lastItemListUpdateTime = Time.time;
    }

    if (Input.GetKeyDown(KeyCode.Delete))
    {
        showMenu = !showMenu;
        Debug.Log("MENU " + showMenu);
        if (!showMenu)
        {
            TryUnlockCamera();
        }
        UpdateCursorState();
    }

    if (Input.GetKeyDown(KeyCode.F5)) Start();

    if (Input.GetKeyDown(KeyCode.F10))
    {
        showMenu = false;
        TryUnlockCamera();
        UpdateCursorState();
        Loader.UnloadCheat();
    }

    if (Input.GetKeyDown(KeyCode.F12)) 
    {
        showDebugMenu = !showDebugMenu;
    }

    // Remove outdated logs efficiently
    debugLogMessages.RemoveAll(msg => Time.time - msg.timestamp > 3f);

    if (showMenu)
    {
        TryLockCamera();
    }
}

        private void TryLockCamera()
        {
            if (InputManager.instance != null)
            {
                Type type = typeof(InputManager);
                FieldInfo field = type.GetField("disableAimingTimer", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    float currentValue = (float)field.GetValue(InputManager.instance);
                    if (currentValue < 2f || currentValue > 10f)
                    {
                        float clampedValue = Mathf.Clamp(currentValue, 2f, 10f);
                        field.SetValue(InputManager.instance, clampedValue);
                    }
                }
                else
                {
                    Debug.LogError("Failed to find field disableAimingTimer.");
                }
            }
            else
            {
                Debug.LogWarning("InputManager.instance not found!");
            }
        }
        private void TryUnlockCamera()
        {
            if (InputManager.instance != null)
            {
                Type type = typeof(InputManager);
                FieldInfo field = type.GetField("disableAimingTimer", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    float currentValue = (float)field.GetValue(InputManager.instance);
                    field.SetValue(InputManager.instance, 0f);
                    Debug.Log("disableAimingTimer reset to 0 (menu closed).");
                }
                else
                {
                    Debug.LogError("Failed to find field disableAimingTimer.");
                }
            }
            else
            {
                Debug.LogWarning("InputManager.instance not found!");
            }
        }

        private void UpdateCursorState()
        {
            Cursor.visible = showMenu;
            Cursor.lockState = showMenu ? CursorLockMode.None : CursorLockMode.Locked;
        }
        private void UpdateItemList()
        {
            DebugCheats.valuableObjects.Clear();
            var valuableArray = UnityEngine.Object.FindObjectsOfType(Type.GetType("ValuableObject, Assembly-CSharp"));
            if (valuableArray != null)
            {
                DebugCheats.valuableObjects.AddRange(valuableArray);
            }

            itemList = ItemTeleport.GetItemList();
            Hax2.Log1($"Lista de itens atualizada: {itemList.Count} itens encontrados.");
        }

        private void UpdateEnemyList()
        {
            enemyNames.Clear();
            enemyList.Clear();

            DebugCheats.UpdateEnemyList();
            enemyList = DebugCheats.enemyList;

            foreach (var enemy in enemyList)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    string enemyName = "Enemy";
                    var enemyParent = enemy.GetComponentInParent(Type.GetType("EnemyParent, Assembly-CSharp"));
                    if (enemyParent != null)
                    {
                        var nameField = enemyParent.GetType().GetField("enemyName", BindingFlags.Public | BindingFlags.Instance);
                        enemyName = nameField?.GetValue(enemyParent) as string ?? "Enemy";
                    }

                    int health = GetEnemyHealth(enemy);
                    string healthText = health >= 0 ? $"HP: {health}" : "HP: Unknown";
                    enemyNames.Add($"{enemyName} [{healthText}]");
                }
            }

            if (enemyNames.Count == 0) enemyNames.Add("No enemies found");
        }
        private void TeleportEnemyToMe()
        {
            if (selectedEnemyIndex < 0 || selectedEnemyIndex >= enemyList.Count)
            {
                Log1($"Índice de inimigo inválido! selectedEnemyIndex={selectedEnemyIndex}, enemyList.Count={enemyList.Count}");
                return;
            }

            var selectedEnemy = enemyList[selectedEnemyIndex];
            if (selectedEnemy == null)
            {
                Log1("Inimigo selecionado é nulo!");
                return;
            }

            try
            {
                GameObject localPlayer = DebugCheats.GetLocalPlayer();
                if (localPlayer == null)
                {
                    Log1("Jogador local não encontrado!");
                    return;
                }

                Vector3 forwardDirection = localPlayer.transform.forward;
                Vector3 targetPosition = localPlayer.transform.position + forwardDirection * 1f + Vector3.up * 1.5f;

                var photonView = selectedEnemy.GetComponent<PhotonView>();
                if (PhotonNetwork.IsConnected && photonView != null && !photonView.IsMine)
                {
                    photonView.RequestOwnership();
                    Log1($"Solicitada posse do inimigo {enemyNames[selectedEnemyIndex]} para garantir controle local.");
                }

                var navMeshAgentField = selectedEnemy.GetType().GetField("NavMeshAgent", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                object navMeshAgent = null;
                if (navMeshAgentField != null)
                {
                    navMeshAgent = navMeshAgentField.GetValue(selectedEnemy);
                    if (navMeshAgent != null)
                    {
                        var enabledProperty = navMeshAgent.GetType().GetProperty("enabled", BindingFlags.Public | BindingFlags.Instance);
                        if (enabledProperty != null)
                        {
                            enabledProperty.SetValue(navMeshAgent, false);
                            Log1($"NavMeshAgent de {enemyNames[selectedEnemyIndex]} desativado para evitar movimento imediato.");
                        }
                    }
                }

                selectedEnemy.transform.position = targetPosition;
                Log1($"Inimigo {enemyNames[selectedEnemyIndex]} teleportado localmente para {targetPosition}");

                Vector3 currentPosition = selectedEnemy.transform.position;
                Log1($"Posição atual do inimigo após teleporte: {currentPosition}");

                if (PhotonNetwork.IsConnected && photonView != null)
                {
                    var enemyType = selectedEnemy.GetType();
                    var teleportMethod = enemyType.GetMethod("EnemyTeleported", BindingFlags.Public | BindingFlags.Instance);
                    if (teleportMethod != null)
                    {
                        teleportMethod.Invoke(selectedEnemy, new object[] { targetPosition });
                        Log1($"Inimigo {enemyNames[selectedEnemyIndex]} teleportado via EnemyTeleported para sincronização multiplayer.");
                    }
                    else
                    {
                        Log1("Método 'EnemyTeleported' não encontrado, sincronização pode não ocorrer.");
                    }
                }

                if (navMeshAgent != null)
                {
                    StartCoroutine(ReEnableNavMeshAgent(navMeshAgent, 2f));
                }

                var enemyGameObject = selectedEnemy.GetComponent<GameObject>(); 
                if (enemyGameObject == null)
                {
                    enemyGameObject = ((MonoBehaviour)selectedEnemy).gameObject;
                }
                if (enemyGameObject != null)
                {
                    enemyGameObject.SetActive(false);
                    enemyGameObject.SetActive(true);
                    Log1($"Inimigo {enemyNames[selectedEnemyIndex]} reativado para forçar renderização.");
                }
                else
                {
                    Log1($"GameObject do inimigo {enemyNames[selectedEnemyIndex]} não encontrado para re-renderização.");
                }

                UpdateEnemyList();
                Log1($"Teleporte de {enemyNames[selectedEnemyIndex]} concluído.");
            }
            catch (Exception e)
            {
                Log1($"Erro ao teleportar inimigo {enemyNames[selectedEnemyIndex]}: {e.Message}");
            }
        }
        private System.Collections.IEnumerator ReEnableNavMeshAgent(object navMeshAgent, float delay)
        {
            yield return new WaitForSeconds(delay);
            var enabledProperty = navMeshAgent.GetType().GetProperty("enabled", BindingFlags.Public | BindingFlags.Instance);
            if (enabledProperty != null)
            {
                enabledProperty.SetValue(navMeshAgent, true);
                Log1("NavMeshAgent reativado após teleporte.");
            }
        }
        private void KillSelectedEnemy()
        {
            if (selectedEnemyIndex < 0 || selectedEnemyIndex >= enemyList.Count)
            {
                Log1("Índice de inimigo inválido!");
                return;
            }

            var selectedEnemy = enemyList[selectedEnemyIndex];
            if (selectedEnemy == null)
            {
                Log1("Inimigo selecionado é nulo!");
                return;
            }

            try
            {
                var healthField = selectedEnemy.GetType().GetField("Health", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (healthField != null)
                {
                    var healthComponent = healthField.GetValue(selectedEnemy);
                    if (healthComponent != null)
                    {
                        var healthType = healthComponent.GetType();
                        var hurtMethod = healthType.GetMethod("Hurt", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (hurtMethod != null)
                        {
                            hurtMethod.Invoke(healthComponent, new object[] { 9999, Vector3.zero });
                            Log1($"Inimigo {enemyNames[selectedEnemyIndex]} ferido com 9999 de dano via Hurt");
                        }
                        else
                            Log1("Método 'Hurt' não encontrado em EnemyHealth");
                    }
                    else
                        Log1("Componente EnemyHealth é nulo");
                }
                else
                    Log1("Campo 'Health' não encontrado em Enemy");

                UpdateEnemyList();
            }
            catch (Exception e)
            {
                Log1($"Erro ao matar inimigo {enemyNames[selectedEnemyIndex]}: {e.Message}");
            }
        }

        private int GetEnemyHealth(Enemy enemy)
        {
            try
            {
                var healthField = enemy.GetType().GetField("Health", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (healthField == null) return -1;

                var healthComponent = healthField.GetValue(enemy);
                if (healthComponent == null) return -1;

                var healthValueField = healthComponent.GetType().GetField("health", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (healthValueField == null) return -1;

                return (int)healthValueField.GetValue(healthComponent);
            }
            catch (Exception e)
            {
                Hax2.Log1($"Erro ao obter vida do inimigo: {e.Message}");
                return -1;
            }
        }

        private void UpdatePlayerList()
        {
            var fakePlayers = playerNames.Where(name => name.Contains("FakePlayer")).ToList();
            var fakePlayerCount = fakePlayers.Count;

            playerNames.Clear();
            playerList.Clear();

            var players = SemiFunc.PlayerGetList();
            foreach (var player in players)
            {
                playerList.Add(player);
                string baseName = SemiFunc.PlayerGetName(player) ?? "Unknown Player";
                bool isAlive = IsPlayerAlive(player, baseName);
                string statusText = isAlive ? "<color=green>[LIVE]</color> " : "<color=red>[DEAD]</color> ";
                playerNames.Add(statusText + baseName);
            }

            for (int i = 0; i < fakePlayerCount; i++)
            {
                playerNames.Add(fakePlayers[i]);
                playerList.Add(null);
            }

            if (playerNames.Count == 0) playerNames.Add("No player Found");
        }

        private void AddFakePlayer()
        {
            int fakePlayerId = playerNames.Count(name => name.Contains("FakePlayer")) + 1;
            string fakeName = $"<color=green>[LIVE]</color> FakePlayer{fakePlayerId}";
            playerNames.Add(fakeName);
            playerList.Add(null);
            Log1($"Added fake player: {fakeName}");
        }

        private bool IsPlayerAlive(object player, string playerName)
        {
            try
            {
                var playerHealthField = player.GetType().GetField("playerHealth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerHealthField == null) return true;

                var playerHealthInstance = playerHealthField.GetValue(player);
                if (playerHealthInstance == null) return true;

                var healthField = playerHealthInstance.GetType().GetField("health", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (healthField == null) return true;

                int health = (int)healthField.GetValue(playerHealthInstance);
                return health > 0;
            }
            catch (Exception e)
            {
                Hax2.Log1($"Erro ao verificar vida de {playerName}: {e.Message}");
                return true;
            }
        }

        private void ReviveSelectedPlayer()
        {
            if (selectedPlayerIndex < 0 || selectedPlayerIndex >= playerList.Count)
            {
                Log1("Índice de jogador inválido!");
                return;
            }
            var selectedPlayer = playerList[selectedPlayerIndex];
            if (selectedPlayer == null)
            {
                Log1("Jogador selecionado é nulo!");
                return;
            }

            try
            {
                var playerDeathHeadField = selectedPlayer.GetType().GetField("playerDeathHead", BindingFlags.Public | BindingFlags.Instance);
                if (playerDeathHeadField != null)
                {
                    var playerDeathHeadInstance = playerDeathHeadField.GetValue(selectedPlayer);
                    if (playerDeathHeadInstance != null)
                    {
                        var inExtractionPointField = playerDeathHeadInstance.GetType().GetField("inExtractionPoint", BindingFlags.NonPublic | BindingFlags.Instance);
                        var reviveMethod = playerDeathHeadInstance.GetType().GetMethod("Revive", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (inExtractionPointField != null)
                        {
                            inExtractionPointField.SetValue(playerDeathHeadInstance, true);
                            Log1("Campo 'inExtractionPoint' definido como true.");
                        }
                        if (reviveMethod != null)
                        {
                            reviveMethod.Invoke(playerDeathHeadInstance, null);
                            Log1("Método 'Revive' chamado com sucesso para: " + playerNames[selectedPlayerIndex]);
                        }
                        else
                        {
                            Log1("Método 'Revive' não encontrado!");
                        }
                    }
                    else
                    {
                        Log1("Instância de playerDeathHead não encontrada.");
                    }
                }
                else
                {
                    Log1("Campo 'playerDeathHead' não encontrado.");
                }

                var playerHealthField = selectedPlayer.GetType().GetField("playerHealth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerHealthField != null)
                {
                    var playerHealthInstance = playerHealthField.GetValue(selectedPlayer);
                    if (playerHealthInstance != null)
                    {
                        var healthType = playerHealthInstance.GetType();
                        var maxHealthField = healthType.GetField("maxHealth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        var healthField = healthType.GetField("health", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        int maxHealth = maxHealthField != null ? (int)maxHealthField.GetValue(playerHealthInstance) : 100;
                        Log1($"maxHealth obtido: {maxHealth}");

                        if (healthField != null)
                        {
                            healthField.SetValue(playerHealthInstance, maxHealth);
                            Log1($"Saúde definida diretamente para {maxHealth} via healthField.");
                        }
                        else
                        {
                            Log1("Campo 'health' não encontrado, tentando HealPlayer como fallback.");
                            Health_Player.HealPlayer(selectedPlayer, maxHealth, playerNames[selectedPlayerIndex]);
                        }

                        int currentHealth = healthField != null ? (int)healthField.GetValue(playerHealthInstance) : -1;
                        Log1($"Saúde atual após revive: {currentHealth}");
                    }
                    else
                    {
                        Log1("Instância de playerHealth é nula, não foi possível restaurar saúde.");
                    }
                }
                else
                {
                    Log1("Campo 'playerHealth' não encontrado, cura não realizada.");
                }
            }
            catch (Exception e)
            {
                Log1($"Erro ao reviver e curar {playerNames[selectedPlayerIndex]}: {e.Message}");
            }
        }

        private void KillSelectedPlayer()
        {
            if (selectedPlayerIndex < 0 || selectedPlayerIndex >= playerList.Count) { Log1("Índice de jogador inválido!"); return; }
            var selectedPlayer = playerList[selectedPlayerIndex];
            if (selectedPlayer == null) { Log1("Jogador selecionado é nulo!"); return; }
            try
            {
                Log1($"Tentando matar: {playerNames[selectedPlayerIndex]} | MasterClient: {PhotonNetwork.IsMasterClient}");
                var photonViewField = selectedPlayer.GetType().GetField("photonView", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (photonViewField == null) { Log1("PhotonViewField não encontrado!"); return; }
                var photonView = photonViewField.GetValue(selectedPlayer) as PhotonView;
                if (photonView == null) { Log1("PhotonView não é válido!"); return; }
                var playerHealthField = selectedPlayer.GetType().GetField("playerHealth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerHealthField == null) { Log1("Campo 'playerHealth' não encontrado!"); return; }
                var playerHealthInstance = playerHealthField.GetValue(selectedPlayer);
                if (playerHealthInstance == null) { Log1("Instância de playerHealth é nula!"); return; }
                var healthType = playerHealthInstance.GetType();
                var deathMethod = healthType.GetMethod("Death", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (deathMethod == null) { Log1("Método 'Death' não encontrado!"); return; }
                deathMethod.Invoke(playerHealthInstance, null);
                Log1($"Método 'Death' chamado localmente para {playerNames[selectedPlayerIndex]}.");
                var playerAvatarField = healthType.GetField("playerAvatar", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (playerAvatarField != null)
                {
                    var playerAvatarInstance = playerAvatarField.GetValue(playerHealthInstance);
                    if (playerAvatarInstance != null)
                    {
                        var playerAvatarType = playerAvatarInstance.GetType();
                        var playerDeathMethod = playerAvatarType.GetMethod("PlayerDeath", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (playerDeathMethod != null) { playerDeathMethod.Invoke(playerAvatarInstance, new object[] { -1 }); Log1($"Método 'PlayerDeath' chamado localmente para {playerNames[selectedPlayerIndex]}."); }
                        else Log1("Método 'PlayerDeath' não encontrado em PlayerAvatar!");
                    }
                    else Log1("Instância de PlayerAvatar é nula!");
                }
                else Log1("Campo 'playerAvatar' não encontrado em PlayerHealth!");
                if (PhotonNetwork.IsConnected && photonView != null)
                {
                    var maxHealthField = healthType.GetField("maxHealth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    int maxHealth = maxHealthField != null ? (int)maxHealthField.GetValue(playerHealthInstance) : 100;
                    Log1(maxHealthField != null ? $"maxHealth encontrado: {maxHealth}" : "Campo 'maxHealth' não encontrado, usando valor padrão: 100");
                    photonView.RPC("UpdateHealthRPC", RpcTarget.AllBuffered, new object[] { 0, maxHealth, true });
                    Log1($"RPC 'UpdateHealthRPC' enviado para todos com saúde=0, maxHealth={maxHealth}, effect=true.");
                    try { photonView.RPC("PlayerDeathRPC", RpcTarget.AllBuffered, new object[] { -1 }); Log1("Tentando RPC 'PlayerDeathRPC' para forçar morte..."); }
                    catch { Log1("RPC 'PlayerDeathRPC' não registrado, tentando alternativa..."); }
                    photonView.RPC("HurtOtherRPC", RpcTarget.AllBuffered, new object[] { 9999, Vector3.zero, false, -1 });
                    Log1("RPC 'HurtOtherRPC' enviado com 9999 de dano para garantir morte.");
                }
                else Log1("Não conectado ao Photon, morte apenas local.");
                Log1($"Tentativa de matar {playerNames[selectedPlayerIndex]} concluída.");
            }
            catch (Exception e) { Log1($"Erro ao tentar matar {playerNames[selectedPlayerIndex]}: {e.Message}"); }
        }

        private void SendSelectedPlayerToVoid()
        {
            if (selectedPlayerIndex < 0 || selectedPlayerIndex >= playerList.Count)
            {
                Log1("Índice de jogador inválido!");
                return;
            }
            var selectedPlayer = playerList[selectedPlayerIndex];
            if (selectedPlayer == null)
            {
                Log1("Jogador selecionado é nulo!");
                return;
            }

            try
            {
                Log1($"Tentando enviar {playerNames[selectedPlayerIndex]} para o void | MasterClient: {PhotonNetwork.IsMasterClient}");

                var photonViewField = selectedPlayer.GetType().GetField("photonView", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (photonViewField == null)
                {
                    Log1("PhotonViewField não encontrado!");
                    return;
                }
                var photonView = photonViewField.GetValue(selectedPlayer) as PhotonView;
                if (photonView == null)
                {
                    Log1("PhotonView não é válido!");
                    return;
                }

                var playerMono = selectedPlayer as MonoBehaviour;
                if (playerMono == null)
                {
                    Log1("selectedPlayer não é um MonoBehaviour!");
                    return;
                }

                var transform = playerMono.transform;
                if (transform == null)
                {
                    Log1("Transform é nulo!");
                    return;
                }

                Vector3 voidPosition = new Vector3(0, -10, 0);
                transform.position = voidPosition;
                Log1($"Jogador {playerNames[selectedPlayerIndex]} enviado localmente para o void: {voidPosition}");

                if (PhotonNetwork.IsConnected && photonView != null)
                {
                    photonView.RPC("SpawnRPC", RpcTarget.AllBuffered, new object[] { voidPosition, transform.rotation });
                    Log1($"RPC 'SpawnRPC' enviado para todos com posição: {voidPosition}");
                }
                else
                {
                    Log1("Não conectado ao Photon, teleporte apenas local.");
                }
            }
            catch (Exception e)
            {
                Log1($"Erro ao enviar {playerNames[selectedPlayerIndex]} para o void: {e.Message}");
            }
        }

public void OnGUI()
{
    if (!showMenu) return;

    // Update UI cache if needed
    if (uiNeedsUpdate)
    {
        GenerateUICache();
        uiNeedsUpdate = false;
    }

    // Draw ESP if enabled
    if (DebugCheats.drawEspBool || DebugCheats.drawItemEspBool || DebugCheats.drawExtractionPointEspBool || DebugCheats.drawPlayerEspBool || DebugCheats.draw3DPlayerEspBool || DebugCheats.draw3DItemEspBool)
    {
        DebugCheats.DrawESP();
    }

    // Cheat Title
    GUI.Label(new Rect(10, 10, 200, 30), "D.A.R.K CHEAT | DEL - MENU");
    GUI.Label(new Rect(198, 10, 200, 30), "MADE BY Github/D4rkks");

    if (showMenu)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Menu Background
        GUIStyle menuStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = MakeSolidBackground(new Color(0.21f, 0.21f, 0.21f), 0.7f) },
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        GUI.Box(new Rect(menuX, menuY, 600, 730), "", menuStyle);

        UIHelper.Begin("D.A.R.K. Menu 1.1.1", menuX, menuY, 600, 800, 30, 30, 10);

        // Menu Dragging
        Rect titleRect = new Rect(menuX, menuY, 600, titleBarHeight);
        if (Event.current.type == EventType.MouseDown && titleRect.Contains(Event.current.mousePosition))
        {
            isDragging = true;
            dragOffset = Event.current.mousePosition - new Vector2(menuX, menuY);
        }
        if (Event.current.type == EventType.MouseUp) isDragging = false;
        if (isDragging && Event.current.type == EventType.MouseDrag)
        {
            Vector2 newPosition = Event.current.mousePosition - dragOffset;
            menuX = Mathf.Clamp(newPosition.x, 0, Screen.width - 600);
            menuY = Mathf.Clamp(newPosition.y, 0, Screen.height - 730);
        }

        // Render Cached UI Elements
        foreach (var uiElement in cachedUIElements)
        {
            if (uiElement.Invoke()) uiNeedsUpdate = true; // Refresh UI on button press
        }

        // Debug Menu (Optimized)
        if (showDebugMenu)
        {
            GUI.Box(new Rect(400, 50, 300, 400), "Debug Logs");
            for (int i = 0; i < debugLogMessages.Count && i < 50; i++) // **Limit logs to 50 entries**
            {
                GUI.Label(new Rect(410, 70 + (i * 20), 280, 20), debugLogMessages[i].message);
            }
        }
    }
}

// **Generates UI Elements Dynamically (Buttons, Sliders, Toggles)**
private void GenerateUICache()
{
    cachedUIElements.Clear();

    // **Player Features**
    cachedUIElements.Add(() => UIHelper.Button("God Mode", menuX + 30, menuY + 80, 280, 30, () => { PlayerController.GodMode(); return true; }));
    cachedUIElements.Add(() => UIHelper.Button("Teleport to Me", menuX + 30, menuY + 120, 280, 30, () => { Teleport.TeleportPlayerToMe(); return true; }));
    cachedUIElements.Add(() => UIHelper.Button("Spawn Items", menuX + 30, menuY + 160, 280, 30, () => { ItemSpawner.SpawnItem(Vector3.zero); return true; }));

    // **Toggles**
    cachedUIElements.Add(() => UIHelper.ButtonBool("Toggle Infinite Health", infiniteHealthActive, menuX + 30, menuY + 200, 280, 30, (newState) => { infiniteHealthActive = newState; Health_Player.MaxHealth(); return true; }));
    cachedUIElements.Add(() => UIHelper.ButtonBool("Toggle Infinite Stamina", stamineState, menuX + 30, menuY + 240, 280, 30, (newState) => { stamineState = newState; PlayerController.MaxStamina(); return true; }));
    cachedUIElements.Add(() => UIHelper.ButtonBool("Toggle God Mode", godModeActive, menuX + 30, menuY + 280, 280, 30, (newState) => { PlayerController.GodMode(); godModeActive = newState; return true; }));

    // **ESP Toggles**
    cachedUIElements.Add(() => UIHelper.ButtonBool("Enable ESP", DebugCheats.drawEspBool, menuX + 30, menuY + 320, 280, 30, (newState) => { DebugCheats.drawEspBool = newState; return true; }));
    cachedUIElements.Add(() => UIHelper.ButtonBool("Enable Item ESP", DebugCheats.drawItemEspBool, menuX + 30, menuY + 360, 280, 30, (newState) => { DebugCheats.drawItemEspBool = newState; return true; }));
    cachedUIElements.Add(() => UIHelper.ButtonBool("Enable Player ESP", DebugCheats.drawPlayerEspBool, menuX + 30, menuY + 400, 280, 30, (newState) => { DebugCheats.drawPlayerEspBool = newState; return true; }));

    // **Speed & Stamina Controls**
    cachedUIElements.Add(() => UIHelper.Slider("Speed", ref sliderValue, 1f, 30f, menuX + 30, menuY + 440, 280, 30, (newValue) => { PlayerController.RemoveSpeed(newValue); return true; }));
    cachedUIElements.Add(() => UIHelper.Slider("Stamina Recharge", ref Hax2.staminaRechargeRate, 1f, 20f, menuX + 30, menuY + 480, 280, 30, (newValue) => { PlayerController.DecreaseStaminaRechargeDelay(Hax2.staminaRechargeDelay, newValue); return true; }));
}

    // Call this when UI elements need updating
    private void RequestUIUpdate()
    {
        uiNeedsUpdate = true;
    }

    // Variables for Optimization
    private bool uiNeedsUpdate = true;
    private List<Func<bool>> cachedUIElements = new List<Func<bool>>();

        private Texture2D MakeSolidBackground(Color color, float alpha)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(color.r, color.g, color.b, alpha));
            texture.Apply();
            return texture;
        }

        public static void Log1(string message) => debugLogMessages.Add(new DebugLogMessage(message, Time.time));

        public class DebugLogMessage
        {
            public string message;
            public float timestamp;
            public DebugLogMessage(string msg, float time) { message = msg; timestamp = time; }
        }
    }
}
