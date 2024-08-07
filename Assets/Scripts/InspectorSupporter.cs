#if UNITY_EDITOR
using UnityEditor;  //유니티 에디터는 빌드에 들어가면 안되니까 에디터일 때에만 코드에 추가할게요!
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorSupporter : MonoBehaviour
{
    byte[,] arounds;
    public byte[,] Arounds => arounds;

    byte[,] states;
    public byte[,] States => states;

    bool[,] mines;
    public bool[,] Mines => mines;

    byte gameState = 0;
    public byte GameState => gameState;

    int leftCover = 0;
    int flag = 0;
    int totalMine = 0;
    int leftMine;
    public int LeftMine => leftMine;

    public void Initiate(int sizeX, int sizeY, int mineAmount)
    {
        if (sizeX == 0 || sizeY == 0) return;

        arounds = new byte[sizeY, sizeX];
        states = new byte[sizeY, sizeX];
        mines = new bool[sizeY, sizeX];

        int totalSize = sizeX * sizeY;
        flag = 0;
        leftCover = totalSize;
        int leftSize = totalSize;
        mineAmount = Mathf.Min(totalSize, mineAmount);
        totalMine = mineAmount;
        leftMine = totalMine;

        for (int y = 0; y < Mines.GetLength(0); y++)
        {
            for (int x = 0; x < Mines.GetLength(1); x++)
            {
                float mineProbability = (float)mineAmount / leftSize;
                bool mineAttached = mineProbability >= Random.value;
                Mines[y, x] = mineAttached;
                if (mineAttached) mineAmount--;
                leftSize--;
            }
        };
        for (int y = 0; y < Mines.GetLength(0); y++)
        {

            for (int x = 0; x < Mines.GetLength(1); x++)
            {
                arounds[y,x] = FindAroundMine(x, y);
            }
        }
        gameState = 1;
    }

    public void OpenBlock(int x, int y)
    {
        if (GameState != 1) return;

        byte targetState = States[y, x];
        if (targetState == 1) return;
        else if (targetState == 0)
        {
            States[y, x] = 2;
            if (mines[y,x])
            {
                gameState = 2;
                return;
            };

            if (arounds[y,x] == 0)
            {
                for (int tempY = Mathf.Max(y - 1, 0); tempY < Mathf.Min(y + 2, arounds.GetLength(0)); tempY++)
                    for (int tempX = Mathf.Max(x - 1, 0); tempX < Mathf.Min(x + 2, arounds.GetLength(1)); tempX++)
                    {
                        OpenBlock(tempX, tempY);
                    }
            };

            leftCover--;

            if (leftCover == totalMine) gameState = 3;
        }
    }
    public void FlagBlock(int x, int y)
    {
        if (GameState != 1) return;

        if (states[y,x] == 0)
        {
            States[y, x] = 1;
            flag++;
        }
        else if (states[y,x] == 1)
        {
            states[y, x] = 0;
            flag--;
        }
    }

    public byte FindAroundMine(int wantX, int wantY)
    {
        byte result = 0;
        for (int y = Mathf.Max(wantY - 1, 0) ; y < Mathf.Min(wantY + 2, arounds.GetLength(0)); y++)
        {
            for (int x = Mathf.Max(wantX - 1, 0) ; x < Mathf.Min(wantX + 2, arounds.GetLength(1)); x++)
            {
                if (mines[y, x]) result++;
            }
        }
        return result;
    }
}

#if UNITY_EDITOR
//유니티 에디터는 빌드에 들어가면 안되니까 에디터일 때에만 코드에 추가할게요!
//이것이 InspectorSupporter의 커스텀 에디터임을 밝힙니다!
[CustomEditor(typeof(InspectorSupporter))]
//[CanEditMultipleObjects] 이건 여러개를 동시에 적용할 때에 대한 내용을 밝혀줘요!
public class InspectorSupporterEditor : Editor
{
    int column = 20;
    int row = 20;
    int num = 75;
    //인스펙터에서 보이는 경우에 대한 내용입니다!
    public override void OnInspectorGUI()
    {
        //이게 여러분들이 원래 보던 인스펙터 창이예요!
        base.OnInspectorGUI();

        //그리고 제가 지금 클릭한 대상을 저장해놓도록 합시다!
        InspectorSupporter drawTarget = (InspectorSupporter)target;

        //대상이 없으면 꺼야해요 ㅜ
        if (drawTarget == null) return;

        //한 줄을 시작해봅시다!
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("행");//행, 열 라벨을 순서대로 적을 거예요
        GUILayout.Label("열");
        EditorGUILayout.EndHorizontal(); //한 줄 끝!

        EditorGUILayout.BeginHorizontal();
        GetNumberFromTextField(ref column); //이건 제가 직접 만든 함수예요! 숫자만 받게 해놓았어요!
        GetNumberFromTextField(ref row);   //행, 열 순서로 숫자를 입력받아봅시다!
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("개수", GUILayout.Width(70)); //개수를 넣는 칸을 표시할 거라 개수라고 써봤는데, 너비를 70으로 맞췄어요!
        GetNumberFromTextField(ref num); //그리고 입력을 받죠!
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("맵 생성")) //버튼을 누르는 것은 이렇게 버튼이라고 쓰고 if문 안에 넣으시면 돼요!
        {
            drawTarget.Initiate(column, row, num);
        }



        string gameStateString = "";
        switch(drawTarget.GameState)
        {
            case 0: gameStateString = "세팅 준비 중.."; break;
            case 1: gameStateString = "찾는 중.."; break;
            case 2: gameStateString = "실패!"; break;
            case 3: gameStateString = "완벽합니다!"; break;
        }
        GUILayout.Label(gameStateString);

        if (drawTarget.Mines == null) return;
        GUILayout.Label($"남은 양 : {drawTarget.LeftMine}");

        for(int y = 0; y < drawTarget.Mines.GetLength(0); y++)
        {
            EditorGUILayout.BeginHorizontal();
            for(int x = 0; x < drawTarget.Mines.GetLength(1); x++)
            {

                switch (drawTarget.States[y, x])
                {
                    case 0:
                        if (GUILayout.Button(" ", GUILayout.Width(20), GUILayout.Height(20)))  //이거는 높이까지 확인해봤어요!
                        {
                            if(Event.current.button == 0) //클릭한 마우스가 왼쪽 클릭이면 0, 오른쪽 클릭이면 1이예요!
                            {
                                drawTarget.OpenBlock(x, y);
                            }
                            else
                            {
                                drawTarget.FlagBlock(x, y);
                            }
                        }
                        break;
                    case 1:
                        if (GUILayout.Button("P", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            if (Event.current.button == 1) drawTarget.FlagBlock(x, y);
                        }
                        break;
                    case 2:
                        int currentArounds = drawTarget.Arounds[y, x];
                        GUILayout.TextArea(drawTarget.Mines[y, x] ? "★" : currentArounds == 0 ? " " : currentArounds.ToString(), GUILayout.Width(20f), GUILayout.Height(20));
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
        };
    }

    void GetNumberFromTextField(ref int targetInt)
    {
        string currentValueText = GUILayout.TextArea(targetInt.ToString());
        if (int.TryParse(currentValueText, out int currentValue)) targetInt = currentValue;
        else if (currentValueText.Length == 0) targetInt = 0;
    }
}
#endif