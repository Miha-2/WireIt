using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerPlacing : MonoBehaviour
{
    [SerializeField] private bool select = false;
    [SerializeField] private Transform selector;
    private RaycastHit _hit;

    private ComponentObject selComp;

    private bool showSelector;

    private void Awake()
    {
        selectorItems = FindObjectsOfType<ItemSelector>()
            .Where(item => item.componentObjectPrefab != null)
            .OrderByDescending(item => item.transform.GetSiblingIndex()).ToArray();
    }

    private void Start() => selector.GetComponent<Renderer>().enabled = showSelector;

    private void Update()
    {
        Placing();

        TryBreak();

        Selector();
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            showSelector = !showSelector;
            selector.GetComponent<Renderer>().enabled = showSelector;
        }
    }


    private void Placing()
    {
        bool hasHit = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity,
            LayerMask.GetMask("Component"));
        _hit = hit;

        selector.gameObject.SetActive(hasHit);

        //If raycast has missed, previously selected component is unSelected
        if (!hasHit)
        {
            if(selComp != null)
                selComp.IsSelected = false;
            selComp = null;
            return;
        }

        Vector3 hitPoint = hit.point - hit.normal * .05f;
        Vector3 squarePos = new(Mathf.RoundToInt(hitPoint.x), Mathf.RoundToInt(hitPoint.y), Mathf.RoundToInt(hitPoint.z));
        Vector3 offset = new(Mathf.RoundToInt(hit.normal.x), Mathf.RoundToInt(hit.normal.y), Mathf.RoundToInt(hit.normal.z));
        selector.position = squarePos + (select ? Vector3.zero : offset);

        //If a new component is selected, their IsSelected statuses a switched
        ComponentObject hitComp = hit.collider.GetComponent<ComponentObject>();
        if (selComp == hitComp)
            return;
        if(selComp != null)
            selComp.IsSelected = false;
        hitComp.IsSelected = true;
        selComp = hitComp;
    }

    private void TryBreak()
    {
        if(!Input.GetMouseButtonDown(0))
            return;
        if(selComp == null)
            return;
        
        if(Circuit.Components.Remove(selComp))
            Destroy(selComp.gameObject);
    }

    [SerializeField] private RectTransform selectorUI;
    private ItemSelector[] selectorItems;
    private int selectedItem;
    private void Selector()
    {
        // Debug.Log();

        selectedItem += (int)Input.mouseScrollDelta.y;
        selectedItem = Mathf.Clamp(selectedItem, 0, selectorItems.Length - 1);
        // selectedItem %= selectorItems.Length;
        // if (selectedItem < 0)
        //     selectedItem += selectorItems.Length;
        
        selectorUI.anchoredPosition = ((RectTransform)selectorItems[selectedItem].transform).anchoredPosition;
    }
    
    private void OnDrawGizmos()
    {
        if(_hit.collider == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_hit.point, .1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_hit.point, _hit.point + _hit.normal * 2);
    }

    private int RoundToGrid(float value) => Mathf.FloorToInt(Mathf.Abs(value)) * (int)Mathf.Sign(value);
}
