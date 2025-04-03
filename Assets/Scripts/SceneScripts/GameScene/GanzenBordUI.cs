﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GanzenBordUI : MonoBehaviour
{
    [Header("References")]
    public GanzenboordManager boardManager;
    public GameObject popupPrefab;
    public Transform goose;
    public Camera mainCamera;
    public Transform levelsParent;

    [Header("Settings")]
    public float cameraSpeed = 5f;
    public float gooseSpeed = 125f;
    public Color completedColor = Color.green;
    public bool debugMode = false;

    private GameObject currentPopup;
    private GameObject canvas;
    private List<GameObject> levelButtons = new();
    private List<Transform> levelRoots = new();
    private Vector3 cameraTarget;
    private int currentLevel = 0;
    private Vector3 gooseOriginalScale;

    public void Start()
    {
        canvas = GameObject.Find("UserHUD");

        Debug.Log("GanzenBordUI started.");
        Debug.Log(boardManager.Appointments);

            //if (debugMode)
            //{
            //    boardManager.SetCompletedLevelCount(boardManager.TotalLevels);
            //}

        foreach (Transform level in levelsParent)
        {
            string buttonName = "Button" + level.name;
            Transform buttonTransform = level.Find(buttonName);
            if (buttonTransform != null)
            {
                GameObject buttonObj = buttonTransform.gameObject;
                levelButtons.Add(buttonObj);
                levelRoots.Add(level);

                int index = levelButtons.Count - 1;
                Button btn = buttonObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnLevelClicked(index));
                }

                if (boardManager.IsLevelCompleted(index))
                {
                    SetLevelColor(index, completedColor);
                }
            }
        }

        if (boardManager.completedLevels > 0)
        {
            int lastIndex = boardManager.completedLevels - 1;
            goose.position = levelButtons[lastIndex].transform.position;
            currentLevel = lastIndex;

            cameraTarget = new Vector3(
                levelButtons[lastIndex].transform.position.x,
                mainCamera.transform.position.y,
                mainCamera.transform.position.z
            );
        }
        else
        {
            goose.position = levelButtons[0].transform.position;
            cameraTarget = mainCamera.transform.position;
        }

        gooseOriginalScale = goose.localScale;

    }

    private void Update()
    {
        // Lees input
        float input = Input.GetAxis("Horizontal");
        if (Mathf.Abs(input) > 0.01f)
        {
            GameObject board = GameObject.Find("GanzenBord");
            if (board != null)
            {
                RectTransform rect = board.GetComponent<RectTransform>();

                float boardWidth = rect.rect.width * rect.lossyScale.x;
                float boardCenter = rect.position.x;
                float minX = boardCenter - boardWidth / 2f;
                float maxX = boardCenter + boardWidth / 2f;

                float oldX = cameraTarget.x;

                cameraTarget.x += input * (cameraSpeed * 50) * Time.deltaTime;
                cameraTarget.x = Mathf.Clamp(cameraTarget.x, minX, maxX);
            }

        }

        Vector3 oldCamPos = mainCamera.transform.position;
        mainCamera.transform.position = Vector3.Lerp(oldCamPos, cameraTarget, Time.deltaTime * cameraSpeed);
    }



    private void OnLevelClicked(int index)
    {
        if (currentPopup) return;

        if (!boardManager.IsLevelUnlocked(index))
        {
            Debug.Log("Level is locked.");
            return;
        }

        cameraTarget = new Vector3(
            levelButtons[index].transform.position.x,
            mainCamera.transform.position.y,
            mainCamera.transform.position.z
        );

        StartCoroutine(MoveGooseToLevel(index));
    }

    private IEnumerator MoveGooseToLevel(int targetIndex)
    {
        int direction = (targetIndex > currentLevel) ? 1 : -1;

        for (int i = currentLevel + direction; i != targetIndex + direction; i += direction)
        {
            Vector3 targetPos = levelButtons[i].transform.position;

            while (Vector3.Distance(goose.position, targetPos) > 0.05f)
            {
                Vector3 moveDir = targetPos - goose.position;

                if (moveDir.x > 0.01f)
                    goose.localScale = new Vector3(Mathf.Abs(gooseOriginalScale.x), gooseOriginalScale.y, gooseOriginalScale.z);
                else if (moveDir.x < -0.01f)
                    goose.localScale = new Vector3(-Mathf.Abs(gooseOriginalScale.x), gooseOriginalScale.y, gooseOriginalScale.z);

                goose.position = Vector3.MoveTowards(goose.position, targetPos, Time.deltaTime * gooseSpeed);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }

        goose.localScale = gooseOriginalScale;
        currentLevel = targetIndex;
        ShowPopup(targetIndex);
    }


    private void ShowPopup(int index)
    {
        if (currentPopup) Destroy(currentPopup);

        currentPopup = Instantiate(popupPrefab, canvas.transform);
        currentPopup.transform.SetAsLastSibling();

        Appointment appointment = boardManager.GetAppointment(index);
        if (appointment == null)
        {
            Debug.LogError("No appointment found for index " + index);
            return;
        }

        PopUpUI popup = currentPopup.GetComponent<PopUpUI>();
        if (popup != null)
        {
            popup.Setup(
                appointment.name,
                appointment.description,
                () => CompleteLevel(index),
                () => Destroy(currentPopup)
            );
        }
    }

    private void CompleteLevel(int index)
    {
        boardManager.MarkLevelCompleted(index);
        SetLevelColor(index, completedColor);
        Destroy(currentPopup);
        Debug.Log($"Level {index + 1} marked as completed.");
    }

    private void SetLevelColor(int index, Color color)
    {
        SpriteRenderer spriteRenderer = levelRoots[index].GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
        else
        {
            Transform button = levelRoots[index].Find("Button" + levelRoots[index].name);
            SpriteRenderer buttonSpriteRenderer = button.GetComponent<SpriteRenderer>();
            buttonSpriteRenderer.color = color;
        }
    }

}