using System.Net.Sockets;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace TextRpg_Test01
{
    internal class Program
    {
        public static Character player;

        private static Armor steelArmor;
        private static Armor leatherArmor;

        private static Weapon oldSword;
        private static Weapon gladius;
        private static Weapon rapier;

        private static Potion lowHpPotion;
        private static Potion lowAtkPotion;
        private static Potion lowDefPotion;

        //장비창(10칸)
        public static List<Item> myInven = new List<Item>(10);
        //장비 효과 수치
        public static int totalItemAtk = 0;
        public static int totalItemDef = 0;
        //기존 장비 인덱스 번호 저장
        public static int exWeaponNum = -1;
        public static int exArmorNum = -1;

        static void Main(string[] args)
        {
            GameDataSetting();
            DisplayGameIntro();
        }

        public static void GameDataSetting()
        {
            // 캐릭터 정보 세팅
            player = new Character("Chad", "전사", 1, 10, 5, 100, 1500);

            // 아이템 정보 세팅
            //방어구
            steelArmor = new Armor("무쇠갑옷", 0, 5, "무쇠로 만들어져 튼튼한 갑옷입니다.");
            leatherArmor = new Armor("가죽갑옷", 0, 2, "소가죽으로 만들어진 낡은 가죽갑옷.");

            //무기
            oldSword = new Weapon("낡은 검", 2, 0, "쉽게 볼 수 있는 낡은 검입니다.");
            gladius = new Weapon("글라디우스", 6, 0, "찌르기에 특화된 사정거리 짧은 한손검.");
            rapier = new Weapon("레이피어", 4, 0, "찌르기에 특화된 사정거리가 긴 한손검.");

            //초기 아이템 세팅
            myInven.Add(steelArmor);
            myInven.Add(oldSword);

            //추가 아이템(포션) : 캐릭터 클래스에 최대 HP 필요
            lowHpPotion = new Potion("하급 체력 포션", 0, 0, "초보 모험자들이 애용하는 물약. 치유량은 미미하다.(HP +3)", 3 );
            lowAtkPotion = new Potion("하급 힘 포션", 2, 0, "저레벨 몬스터들을 재료로 고아낸 힘 물약.(Atk +2)", 0); ;
            lowDefPotion = new Potion("하급 방어력 포션", 0, 2, "일부 몬스터들의 갑각을 재료로 고아낸 물약. 사용 후 피부가 갈라진다.(Def +2)", 0);
        }
        static void DisplayGameIntro()
        {
            Console.Clear();

            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 전전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("1");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(". 상태보기");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("2");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(". 인벤토리");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("3");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(". 상점 방문");
            Console.WriteLine();

            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(1, 3);
            switch (input)
            {
                case 1:
                    DisplayMyInfo();
                    break;

                case 2:
                    DisplayInventory();
                    break;
                case 3:
                    GoStartingShop();
                    break;
            }
        }
        static void DisplayMyInfo()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상태보기");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("캐릭터의 정보를 표시합니다.");
            Console.WriteLine();

            Console.Write("Lv.");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(player.Level);
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"{player.Name}({player.Job})");

            Console.Write("공격력 : ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(player.Atk);
            Console.ForegroundColor = ConsoleColor.White;
            if (totalItemAtk != 0)
            {
                //무기 장착하면
                Console.Write('(');
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"+{totalItemAtk}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(')');
            }
            Console.WriteLine();

            Console.Write("방어력 : ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(player.Def);
            Console.ForegroundColor = ConsoleColor.White;
            if (totalItemDef != 0)
            {
                //갑옷 장착하면
                Console.Write('(');
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"+{totalItemDef}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(')');
            }
            Console.WriteLine();

            Console.Write("체력   : ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(player.Hp);
            Console.ForegroundColor = ConsoleColor.White;


            Console.Write("Gold   : ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(player.Gold);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" G");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("0");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(". 나가기");

            int input = CheckValidInput(0, 0);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
            }
        }
        static void DisplayInventory()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("인벤토리");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            int i = myInven.Count;
            int j = 0;

            while (j < i)
            {
                Console.Write($"-   {myInven[j].Item_Name}");
                Console.ForegroundColor = ConsoleColor.White;
                if (myInven[j].Item_Atk == 0)
                {
                    //if Item == armor
                    Console.Write("    | 방어력 ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(myInven[j].Item_Def);
                }
                else
                {
                    //if item == weapon
                    Console.Write("    | 공격력 ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(myInven[j].Item_Atk);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($" | {myInven[j].Item_Discription}");

                j++;
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("0");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(".나가기");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("1");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(".장착관리");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            int input = CheckValidInput(0, 1);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
                case 1:
                    DisplayEquipItems();
                    break;
            }
        }
        public static void DisplayEquipItems()
        {
            int i = myInven.Count;
            int j = 0;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("인벤토리 - 장착관리");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("아이템에 해당하는 숫자를 입력하시면 장착, 장착된 아이템의 숫자를 입력하시면 해제됩니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            while (j < i)
            {
                Console.Write("-  ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(j + 1);
                Console.ForegroundColor = ConsoleColor.White;

                if (myInven[j].Item_Atk == 0)
                {
                    //if Item == armor
                    Console.Write($" {myInven[j].Item_Name}    | 방어력 ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(myInven[j].Item_Def);
                }
                else
                {
                    //if item == weapon
                    Console.Write($" {myInven[j].Item_Name}    | 공격력 ");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(myInven[j].Item_Atk);
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($" | {myInven[j].Item_Discription}");

                j++;
            }
            Console.WriteLine("");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("0");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(".나가기");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("1~");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(".아이템 장착/해제");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            int input = CheckValidInput(0, i);
            switch (input)
            {
                case 0:
                    DisplayInventory();
                    break;
                case 1:
                    EquipItem(0);
                    DisplayInventory();
                    break;
                case 2:
                    EquipItem(1);
                    DisplayInventory();
                    break;
                case 3:
                    EquipItem(2);
                    DisplayInventory();
                    break;
                case 4:
                    EquipItem(3);
                    DisplayInventory();
                    break;
                case 5:
                    EquipItem(4);
                    DisplayInventory();
                    break;
                case 6:
                    EquipItem(5);
                    DisplayInventory();
                    break;
                case 7:
                    EquipItem(6);
                    DisplayInventory();
                    break;
                case 8:
                    EquipItem(7);
                    DisplayInventory();
                    break;
                case 9:
                    EquipItem(8);
                    DisplayInventory();
                    break;
                case 10:
                    EquipItem(9);
                    DisplayInventory();
                    break;
            }
        }
        public static void GoStartingShop()
        {
            Console.Clear();
            Console.WriteLine();

            //아래 무시
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("0");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(". 나가기");

            int input = CheckValidInput(0, 0);
            switch (input)
            {
                case 0:
                    DisplayGameIntro();
                    break;
            }
        }
        public static void EquipItem(int i)
        {
            bool isEquiped = myInven[i].Item_Name.Contains("[E]");

            if (isEquiped == true)
            {
                myInven[i].Item_Name = myInven[i].Item_Name.Replace("[E]", "");

                //if 해제
                if (myInven[i].Item_Atk == 0)
                {
                    //갑옷 해제
                    player.Def = player.Def - myInven[i].Item_Def;
                }
                else if (myInven[i].Item_Def == 0)
                {
                    //무기 해제
                    player.Atk = player.Atk - myInven[i].Item_Atk;
                }
            }
            else
            {
                myInven[i].Item_Name = "[E]" + myInven[i].Item_Name;
                //갑옷장착
                if (myInven[i].Item_Atk == 0)
                {
                    //기존 장착한거 없으면
                    if (exArmorNum == -1)
                    {
                        player.Def = player.Def + myInven[i].Item_Def;
                        totalItemDef = myInven[i].Item_Def;
                        exArmorNum = i;
                    }
                    else
                    {
                        player.Def = player.Def - myInven[exArmorNum].Item_Def + myInven[i].Item_Def;
                        //기존 장착템[E] 지우기
                        myInven[exArmorNum].Item_Name = myInven[exArmorNum].Item_Name.Replace("[E]", ""); ;
                        totalItemDef = myInven[i].Item_Def;
                        exArmorNum = i;
                    }
                }
                else if (myInven[i].Item_Def == 0)
                {
                    if (exWeaponNum == -1)
                    {
                        player.Atk = player.Atk + myInven[i].Item_Atk;
                        totalItemAtk = myInven[i].Item_Atk;
                        exWeaponNum = i;
                    }
                    else
                    {
                        player.Atk = player.Atk - myInven[exWeaponNum].Item_Atk + myInven[i].Item_Atk;
                        myInven[exWeaponNum].Item_Name = myInven[exWeaponNum].Item_Name.Replace("[E]", ""); ;
                        totalItemAtk = myInven[i].Item_Atk;
                        exWeaponNum = i;
                    }
                }
            }
        }
        static int CheckValidInput(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                        return ret;
                }

                Console.WriteLine("잘못된 입력입니다.");
            }
        }
    }

    public class Item
    {
        public string Item_Name { get; set; }
        public int Item_Atk { get; set; }
        public int Item_Def { get; set; }
        public string Item_Discription { get; set; }
        public Item(string _item_name, int _item_atk, int _item_def, string _item_discription)
        {
            Item_Name = _item_name;
            Item_Atk = _item_atk;
            Item_Def = _item_def;
            Item_Discription = _item_discription;
        }
    }

    public class Weapon : Item
    {
        public Weapon(string _item_name, int _item_atk, int _item_def, string _item_discription) : base(_item_name, _item_atk, _item_def, _item_discription)
        {
            Item_Name = _item_name;
            Item_Atk = _item_atk;
            Item_Def = 0;
            Item_Discription = _item_discription;
        }
    }

    public class Armor : Item
    {
        public Armor(string _item_name, int _item_atk, int _item_def, string _item_discription) : base(_item_name, _item_atk, _item_def, _item_discription)
        {
            Item_Name = _item_name;
            Item_Atk = 0;
            Item_Def = _item_def;
            Item_Discription = _item_discription;
        }
    }

    public class Potion : Item
    {
        public int Potion_Healing;
        public Potion(string _potion_name, int _potion_atk, int _potion_def, string _potion_discription, int _potion_Healing) : base(_potion_name, _potion_atk, _potion_def, _potion_discription)
        {
            Item_Name = _potion_name;
            Item_Atk = _potion_atk;
            Item_Def = _potion_def;
            Item_Discription = _potion_discription;
            Potion_Healing = _potion_Healing;
        }
    }

    public class Character
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int Hp { get; }
        public int Gold { get; }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
        }
    }

}