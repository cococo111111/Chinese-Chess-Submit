﻿using UnityEngine;
using System.Collections;
using System.Threading;
public class ChessControl : MonoBehaviour {
    public static ChessControl instance;

    public  int FromX = -1, FromY = -1, ToX = -1, ToY = -1;//存储位置移动的起点和终点信息
	
	public  string NextPlayerTipStr="";
	public  bool ChessMove =true;//true   redMove   false BlackMove
	public  bool TrueOrFalse=true;//判断这个时候输赢状态能否走棋  //重新开始记得该true
	public  string RedName=null,BlackName=null,ItemName;//blackchessname  and   redchessname
	int bestmove;
    public MoveSetting.CHESSMOVE chere;
	GameObject renji;//识别人机走到哪里
	public static bool posthread=true;//判断线程里面的内容是否执行完毕
    public Transform BlackSelectChess, RedSelectChess;
	
	//得到点击的象棋名字
	//判断点击到的是什么？
	//0是空   1 是黑色   2 是红色
    void Start()
    {
        instance = this;
    }
	public int IsBlackOrRed(int x,int y){
        int Count = board.instance.chess[y, x];
		if (Count == 0)
			return 0;
		else if (Count > 0 && Count < 8)//是黑色
			return 1;
		else  //是红色 
			return 2;
	}
	public void BlackNameOrRedName(GameObject obj){//得到棋子的名字
	if (obj.name.Substring (0, 1) == "r")
			RedName = obj.name;//得到red名字
		else if (obj.name.Substring (0, 1) == "b")
			BlackName = obj.name;//得到black名字
		else 
			ItemName = obj.name;//得到item名字
	}
	//移动
	public void IsMove(string One,GameObject game ,int x1,int y1,int x2,int y2){
	    GameObject parent1 = GameObject.Find (One);
		parent1.transform.SetParent(game.transform);
		parent1.transform.localPosition = Vector3.zero;
        board.instance.chess[y2, x2] = board.instance.chess[y1, x1];
        board.instance.chess[y1, x1] = 0;
	}
	//吃子
	public void IsEat(string Frist,string sconde,int x1,int y1,int x2,int y2)
    {
	    GameObject Onename = GameObject.Find (Frist);//得到第一个
		GameObject Twoname = GameObject.Find (sconde);//得到第二个名字
		GameObject Twofather = Twoname.gameObject.transform.parent.gameObject;//得到第二个的父亲
		Onename.gameObject.transform.SetParent(Twofather.transform);
		Onename.transform.localPosition = Vector3.zero;
        board.instance.chess[y2, x2] = board.instance.chess[y1, x1];
        board.instance.chess[y1, x1] = 0;
        GameObject a = GameObject.Find("xiaoshi");
        Twoname.transform.SetParent(a.transform);
		Twoname.transform.localPosition = new Vector3(5000,5000,0);
	}
	//用来悔棋功能
	//点击事件
	//播放音乐
    public void IsClickCheck(GameObject obj)
    {
        renji = GameObject.Find("chessRobot");
		if (TrueOrFalse == false)
			return;       
		BlackNameOrRedName (obj);//是否点击到棋子  如果是  就得到棋子
		if (obj.name.Substring (0, 1) != "i")
			obj = obj.gameObject.transform.parent.gameObject;//得到他的父容器
		int x=System.Convert.ToInt32((obj.transform.localPosition.x)/43);
		int y = System.Convert.ToInt32(Mathf.Abs((obj.transform.localPosition.y)/43));
		int Result = IsBlackOrRed (x, y);//判断点击到了什么
		switch (Result) {
		case 0://点击到了空  是否要走棋
			//如果点击到了空格  就把对象清空
			//p.MusicPlay();
			for(int i=1;i<=90;i++)
			{
				GameObject Clear = GameObject.Find("prefabs"+i.ToString());
				Destroy(Clear);
			}
			if(posthread ==false)
				return ;
			ToY = y;
			ToX = x;
			if(ChessMove){//红色走
				if(RedName == null)
					return;
                //string sssRed = RedName;//记录红色棋子的名字
                bool ba = rules.IsValidMove(board.instance.chess, FromX, FromY, ToX, ToY);
			if(!ba)
					return;

            int a = board.instance.chess[FromY, FromX];
            int b = board.instance.chess[ToY, ToX];
                BackStepChess.instance.AddChess(BackStepChess.Count, FromX, FromY, ToX, ToY, true, a, b);
				IsMove(RedName,obj,FromX,FromY,ToX,ToY);//走了
                NextPlayerTipStr = "黑方走";
				KingPosition.JiangJunCheck();
				ChessMove = false;
				//getString();
                if (NextPlayerTipStr == "红色棋子胜利")
					return ;//因为没有携程关系  每次进入黑色走棋的时候都判断 棋局是否结束
                if (BtnControl.ChessPeople == 2)
				{//如果现在是双人对战模式
					BlackName = null;
					RedName = null;                    
					return;
				}
                if (ChessMove == false) {
                    Invoke("threm", 0.2f);
                }
			//执行走棋

			}
			else{//黑色走
				if(BlackName==null)
					return;
                bool ba = rules.IsValidMove(board.instance.chess, FromX, FromY, ToX, ToY);
				if(!ba)
					return;
                int a = board.instance.chess[FromY, FromX];
                int b = board.instance.chess[ToY, ToX];
                BackStepChess.instance.AddChess(BackStepChess.Count, FromX, FromY, ToX, ToY, true, a, b);
				//看看是否能播放音乐
				IsMove(BlackName,obj,FromX,FromY,ToX,ToY);
			
				//黑色走棋
				ChessMove = true;
                NextPlayerTipStr = "红方走";
				KingPosition.JiangJunCheck();
			}
			break;
		case 1://点击到了黑色  是否选中  还是  红色要吃子
			if(posthread ==false)
				return ;
			if(!ChessMove){
				FromX = x;
				FromY = y;
                if (BlackSelectChess != null)
                {
                    BlackSelectChess.GetChild(0).gameObject.SetActive(false);
                }
                BlackSelectChess = obj.transform.GetChild(0);
                BlackSelectChess.GetChild(0).gameObject.SetActive(true);
				for(int i=1;i<=90;i++)
				{
					GameObject Clear = GameObject.Find("prefabs"+i.ToString());
					Destroy(Clear);
				}
				MoveSetting.instance.ClickChess(FromX,FromY);
			}
			else{
				for(int i=1;i<=90;i++)
				{
					GameObject Clear = GameObject.Find("prefabs"+i.ToString());
					Destroy(Clear);
				}
			if(RedName ==null)
					return;
				ToX = x;
				ToY = y;
                bool ba = rules.IsValidMove(board.instance.chess, FromX, FromY, ToX, ToY);
				if(!ba)
					return;
                int a = board.instance.chess[FromY, FromX];
                int b = board.instance.chess[ToY, ToX];
                BackStepChess.instance.AddChess(BackStepChess.Count, FromX, FromY, ToX, ToY, true, a, b);
				//看看是否能播放音乐
				IsEat(RedName,BlackName,FromX,FromY,ToX,ToY);
				ChessMove = false;
				//红色吃子  变黑色走
                NextPlayerTipStr = "黑方走";
				KingPosition.JiangJunCheck();
                if (NextPlayerTipStr == "红色棋子胜利")
					return ;//因为没有携程关系  每次进入黑色走棋的时候都判断 棋局是否结束
                if (BtnControl.ChessPeople == 2)
                {
					RedName=null;
					BlackName=null;
					return;
				}
                if (ChessMove == false) { 
                    //threm();
                    Invoke("threm", 0.2f);
                }
            }
			break;
		case 2://点击到了红色   是否选中  还是黑色要吃子                                            
			if(posthread ==false)
				return ;
            if (RedSelectChess != null)
            {
                RedSelectChess.GetChild(0).gameObject.SetActive(false);
            }
            RedSelectChess = obj.transform.GetChild(0);
            RedSelectChess.GetChild(0).gameObject.SetActive(true);
			if(ChessMove){
				FromX=x;
				FromY = y;
				for(int i=1;i<=90;i++)
				{
					GameObject Clear = GameObject.Find("prefabs"+i.ToString());
					Destroy(Clear);
				}
                MoveSetting.instance.ClickChess(FromX, FromY);
                
			}
			else{
				for(int i=1;i<=90;i++)
				{
					GameObject Clear = GameObject.Find("prefabs"+i.ToString());
					Destroy(Clear);
				}
				if(BlackName==null)
					return;
					ToX = x;
					ToY = y;
                    bool ba = rules.IsValidMove(board.instance.chess, FromX, FromY, ToX, ToY);
				if(!ba)
					return;
                int a = board.instance.chess[FromY, FromX];
                int b = board.instance.chess[ToY, ToX];
                BackStepChess.instance.AddChess(BackStepChess.Count, FromX, FromY, ToX, ToY, true, a, b);
				//看看是否能播放音乐
				IsEat(BlackName,RedName,FromX,FromY,ToX,ToY);
                
				RedName = null;
				BlackName = null;
				ChessMove = true;
                NextPlayerTipStr = "红方走";
				KingPosition.JiangJunCheck();                
			}
			break;
	
		}
	
	}
	public void threm(){
		//str="对方正在思考";

        if (ChessMove == false)
        {

            chere = SearchEngine.instance.SearchAGoodMove(board.instance.chess);
            string s1 = "";
            string s2 = "";
            string s3 = "";
            string s4 = "";
            s1 = SearchEngine.instance.Itemfirname(chere);
            s2 = SearchEngine.instance.Itemsconname(chere);
            GameObject one = GameObject.Find(s1);
            GameObject two = GameObject.Find(s2);
            foreach (Transform child in one.transform)
                s3 = child.name;//第一个象棋名字
            foreach (Transform child in two.transform)
                s4 = child.name;//吃到的子的象棋名字

            //将AI走的这一步加入棋走的每一步列表
            if (s4 == "")
            {
                int a = board.instance.chess[chere.From.y, chere.From.x];
                int b = board.instance.chess[chere.To.y, chere.To.x];
                BackStepChess.instance.AddChess(BackStepChess.Count, chere.From.x, chere.From.y, chere.To.x, chere.To.y, false, a, b);
                IsMove(s3, two, chere.From.x, chere.From.y, chere.To.x, chere.To.y);
                renji.transform.localPosition = one.transform.localPosition;

            }
            else
            {
                int a = board.instance.chess[chere.From.y, chere.From.x];
                int b = board.instance.chess[chere.To.y, chere.To.x];
                BackStepChess.instance.AddChess(BackStepChess.Count, chere.From.x, chere.From.y, chere.To.x, chere.To.y, false, a, b);
                IsEat(s3, s4, chere.From.x, chere.From.y, chere.To.x, chere.To.y);
                renji.transform.localPosition = one.transform.localPosition;

            }

            RedName = null;
            BlackName = null;
            if (BlackSelectChess!=null)
	        {
		        BlackSelectChess.transform.GetChild(0).gameObject.SetActive(false);
	        }
            BlackSelectChess = GameObject.Find(s3).transform;
            BlackSelectChess.GetChild(0).gameObject.SetActive(true);
            NextPlayerTipStr = "红方走";
            KingPosition.JiangJunCheck();
            ChessMove = true;
        }

		}

}