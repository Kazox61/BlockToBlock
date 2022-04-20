using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PublicLevelsUIManager : MonoBehaviour {
    [SerializeField] private GameObject contentHolder, scrollView, panelSearch;
    [SerializeField] private GameObject tabFocus1, tabFocus2, tabFocus3;
    [SerializeField] private TMP_Text textTab1, textTab2, textTab3;
    [SerializeField] private TMP_InputField inputSearchText;
    [SerializeField] private MainMenuUIManager mainMenuUIManager;
    private LootLockerManager lootLockerManager;
    [SerializeField] private Color unfocusedColor;
    private UserInfoManager userInfoManager;
    [SerializeField] private ManagerAnchor managerAnchor;

    private SearchMode searchMode;

    public void Awake() {
        if (managerAnchor.IsSet) {
            SetManagers(managerAnchor.Item);
        }
    }

    public void SetManagers(ManagerData data) {
        lootLockerManager = data.lootLockerManager;
        userInfoManager = data.userInfoManager;
    }


    public void OnTabClicked(int i) {
        mainMenuUIManager.DeleteChildObjects(contentHolder);
        tabFocus1.SetActive(false);
        tabFocus2.SetActive(false);
        tabFocus3.SetActive(false);
        textTab1.color = unfocusedColor;
        textTab2.color = unfocusedColor;
        textTab3.color = unfocusedColor;

        if (i == 1) {
            tabFocus1.SetActive(true);
            textTab1.color = Color.white;
            panelSearch.SetActive(false);
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollView.GetComponent<RectTransform>().sizeDelta.x, 530);
            lootLockerManager.ShowGlobalLevelsSortedByLikes();
        }
        else if (i == 2) {
            tabFocus2.SetActive(true);
            textTab2.color = Color.white;
            panelSearch.SetActive(true);
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollView.GetComponent<RectTransform>().sizeDelta.x, 400);
            searchMode = SearchMode.ID;
        }
        else if (i == 3) {
            tabFocus3.SetActive(true);
            textTab3.color = Color.white;
            panelSearch.SetActive(true);
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollView.GetComponent<RectTransform>().sizeDelta.x, 400);
            searchMode = SearchMode.Levelname;
        }
    }

    public void OnSearchClicked() {
        mainMenuUIManager.DeleteChildObjects(contentHolder);
        if (searchMode.Equals(SearchMode.ID)) {
            lootLockerManager.SearchLevelByID(inputSearchText.text);
        }
        else {
            lootLockerManager.SearchLevelByLevelname(inputSearchText.text);
        }
    }

    public void VoteLike(BoxPublicLevel box) {
        if (!box.voteStatus.Equals(VoteStatus.none)) return;
        box.voteStatus = VoteStatus.like;
        box.imageBGLike.sprite = lootLockerManager.spriteVoteLike;
        userInfoManager.UserInfo.votes.Add(box.id, true);
        //lootLockerManager.UploadVotesToAsset(box);
    }

    public void VoteDislike(BoxPublicLevel box) {
        if (!box.voteStatus.Equals(VoteStatus.none)) return;
        box.voteStatus = VoteStatus.dislike;
        box.imageBGDislike.sprite = lootLockerManager.spriteVoteDislike;
        userInfoManager.UserInfo.votes.Add(box.id, false);
        //@TODO Upload Vote To Asset
    }
}

public enum SearchMode {
    ID,
    Levelname
}
