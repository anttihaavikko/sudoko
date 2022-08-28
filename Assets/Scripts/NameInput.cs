using System;
using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Visuals;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class NameInput : MonoBehaviour
{
    public TMP_InputField field;
    public EffectCamera cam;
    [SerializeField] private Character player;
    [SerializeField] private List<Appearer> appearers;

    private bool done;

    private void Start()
    {
        field.onValueChanged.AddListener(ToUpper);
        Invoke(nameof(FocusInput), 0.6f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Save();
        }
    }

    private void FocusInput()
	{
        EventSystem.current.SetSelectedGameObject(field.gameObject, null);
        field.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void ToUpper(string value)
    {
        field.text = value;
    }

    public void Save()
    {
        if (done || string.IsNullOrEmpty(field.text)) return;
        player.WalkTo(10f, 0, false);
        PlayerPrefs.SetString("PlayerName", field.text);
        PlayerPrefs.SetString("PlayerId", Guid.NewGuid().ToString());
        Invoke(nameof(ChangeScene), 1.5f);
        appearers.ForEach(a => a.HideWithDelay());
        done = true;
    }

    private void ChangeScene()
    {
        SceneChanger.Instance.ChangeScene("Map");
    }
}
