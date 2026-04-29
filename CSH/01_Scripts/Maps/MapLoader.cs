/*
using PSB.Code.BattleCode.Enemies.BTs.Actions;
using System.Collections.Generic;
using UnityEngine;
using Work.CSH.Scripts.Maps;

namespace Work.CSH.Scripts
{
    public enum WallState
    {
        Close = 0, Open = 1
    }
    public enum RoomType
    {
        Empty, Start = 1, Boss = 2, Shop, Treasure, Battle, End
    }
    public struct RoomInfo
    {
        public RoomType type;
        public WallState up;
        public WallState down;
        public WallState left;
        public WallState right;

        
    }
    public class MapLoader : MonoBehaviour
    {
        public GameObject[] roomPrefabs;

        [SerializeField] private int mapWidth;
        [SerializeField] private int mapHeight;
        [SerializeField] private float roomPaddingX = 12f;
        [SerializeField] private float roomPaddingY = 6f;
        [SerializeField] private int targetPointCount;

        private List<Transform> battleRoomList;
        private List<Vector2> specialRoomPos;

        private RoomInfo[,] mapArray;
        private Vector2[] dirs = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };
        private List<Vector2> occupiedPos = new List<Vector2>();

        private void Awake()
        {
            
        }

        [ContextMenu("Load Map")]
        public void LoadMap()
        {
            mapArray = new RoomInfo[mapHeight+1, mapWidth+1];
            specialRoomPos = new List<Vector2>();
            Vector2 startPoint = new Vector2(Random.Range(1, mapHeight-1), Random.Range(1, mapWidth - 1));
            mapArray[(int)startPoint.y, (int)startPoint.x].type = RoomType.Start;
            for (int i = 0; i < targetPointCount; i++)
            {
                Vector2 pos = Vector2.zero;
                pos = new Vector2(Random.Range(0, mapHeight), Random.Range(0, mapWidth));
                while (startPoint == pos || specialRoomPos.Contains(pos))
                {
                    pos = new Vector2(Random.Range(0, mapHeight), Random.Range(0, mapWidth));
                    
                }
                //mapArray[(int)pos.y, (int)pos.x].type = (RoomType)i;
                mapArray[(int)pos.y, (int)pos.x].type = (RoomType.Boss);
                specialRoomPos.Add(pos);
            }

            foreach (var pos in specialRoomPos)
            {
                
                CreateRandomRoad(startPoint, RandomDir(startPoint), pos);
            }
            
            for(int i = 0; i < mapHeight; i++)
            {
                for(int j = 0; j < mapWidth; j++)
                {
                    RoomInfo roomInfo = mapArray[i, j];
                    Room room;
                    if(roomInfo.type == RoomType.Empty)
                    {
                        continue;
                    }
                    else if (roomInfo.type == RoomType.Battle || roomInfo.type == RoomType.Start)
                    {
                        room = Instantiate(roomPrefabs[0], new Vector3(j * roomPaddingX, i * roomPaddingY, 0), Quaternion.identity, transform).GetComponent<Room>();

                    }
                    else
                    {
                        room = Instantiate(roomPrefabs[5], new Vector3(j * roomPaddingX, i * roomPaddingY, 0), Quaternion.identity, transform).GetComponent<Room>();
                    }
                    GameObject up = roomPrefabs[3 + (int)roomInfo.up];
                    GameObject down = roomPrefabs[3 + (int)roomInfo.down];
                    GameObject left = roomPrefabs[1 + (int)roomInfo.left];
                    GameObject right = roomPrefabs[1 + (int)roomInfo.right];
                    room.CreateWall(up, down, left, right);
                    room.SetOrderInLayer((mapWidth - j) * (1 + i));
                }
            }
        }

        private Vector2 RandomDir(Vector2 pos)
        {
            List<Vector2> ds = new() { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            foreach (var d in dirs)
            {
                Vector2 nextPos = pos + d;
                if (nextPos.x < 0 || nextPos.x >= mapWidth || nextPos.y < 0 || nextPos.y >= mapHeight)
                {
                    ds.Remove(d);

                    continue;
                }

                
            }
            Debug.Log($"Possible Dirs Count: {ds.Count}");
            Vector2 dir = ds[Random.Range(0, ds.Count)];
            return dir;
        }

        public void CreateRandomRoad(Vector2 pos, Vector2 dir, Vector2 target)
        {

            Debug.Log($"Pos: {pos}, Dir: {dir}, Target: {target}");
            pos += dir;

            switch (dir)
            {
                case Vector2 v when v == Vector2.up:
                    mapArray[(int)pos.y, (int)pos.x].up = WallState.Open;
                    mapArray[(int)(pos.y - 1), (int)pos.x].down = WallState.Open;
                    break;
                case Vector2 v when v == Vector2.down:
                    mapArray[(int)pos.y, (int)pos.x].down = WallState.Open;
                    mapArray[(int)pos.y + 1, (int)pos.x].up = WallState.Open;
                    break;
                case Vector2 v when v == Vector2.left:
                    mapArray[(int)pos.y, (int)pos.x].right = WallState.Open;
                    mapArray[(int)pos.y, (int)pos.x + 1].left = WallState.Open;
                    break;
                case Vector2 v when v == Vector2.right:
                    mapArray[(int)pos.y, (int)pos.x].left = WallState.Open;
                    mapArray[(int)pos.y, (int)pos.x - 1].right = WallState.Open;
                    break;
            }

            if (pos == target)
            {
                return;
            }
            mapArray[(int)pos.y, (int)pos.x].type = RoomType.Battle;
            
            Vector2 nextDir = -dir;
            
            while(nextDir == -dir)
            {
                nextDir = RandomDir(pos);
            }
            CreateRandomRoad(pos, nextDir, target);


        }
    }
}
*/

/*
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Work.CSH.Scripts.Maps;

namespace Work.CSH.Scripts
{
    public enum WallState { Close = 0, Open = 1 }

    public enum RoomType
    {
        Empty = 0,
        Start = 1,
        Boss = 2,
        Shop = 3,
        Treasure = 4,
        Battle = 5,
        End = 6
    }

    public struct RoomInfo
    {
        public RoomType type;
        public WallState up;
        public WallState down;
        public WallState left;
        public WallState right;
        public GameObject Object;

        public void InitAsClosed()
        {
            up = WallState.Close;
            down = WallState.Close;
            left = WallState.Close;
            right = WallState.Close;
        }
    }

    public class MapLoader : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject[] roomPrefabs;

        [Header("Grid")]
        [SerializeField] private int mapWidth = 5;    // x
        [SerializeField] private int mapHeight = 5;    // y

        [Header("World Spacing")]
        [SerializeField] private float roomPaddingX = 12f;
        [SerializeField] private float roomPaddingY = -11f;

        [Header("Points")]
        [SerializeField] private int targetPointCount = 2;  // 특수방 개수(보스 등)
        [SerializeField] private int maxBigRoomCount = 2;    // (방 통합으로 생기는)크기 2x2짜리 방 개수

        [Header("Road")]
        [Range(0f, 1f)] [SerializeField] private float towardTargetBias = 0.65f; // 0~1, 높을수록 목표로 더 직진
        [Range(0f, 1f)] [SerializeField] private float noBacktrackBias = 0.85f;  // 직전 방향 반대로 가는 확률을 얼마나 줄일지
        [SerializeField] private int maxStepsMultiplier = 3;     // 안전장치

        private RoomInfo[,] mapArray;
        private List<Vector2Int> specialRoomPos;

        private static readonly Vector2Int[] Dir4 =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        private void Awake()
        {
            Mathf.PerlinNoise(0f, 0f);
        }
        


        [ContextMenu("Load Map")]
        public void LoadMap()
        {
            // 0) 초기화
            mapArray = new RoomInfo[mapHeight, mapWidth];
            specialRoomPos = new List<Vector2Int>();

            for (int y = 0; y < mapHeight; y++)
                for (int x = 0; x < mapWidth; x++)
                {
                    mapArray[y, x].type = RoomType.Empty;
                    mapArray[y, x].InitAsClosed();
                }

            // 1) 시작 방 배치 (테두리 피하기)
            Vector2Int start = new Vector2Int(
                Random.Range(1, mapWidth - 1),
                Random.Range(1, mapHeight - 1)
            );
            SetRoomType(start, RoomType.Start);

            // 2) 특수 방(보스 등) 배치
            for (int i = 0; i < targetPointCount; i++)
            {
                Vector2Int p = RandomEmptyCellExcept(start);
                SetRoomType(p, RoomType.Boss); // 일단 전부 Boss로
                specialRoomPos.Add(p);
            }

            // 3) 시작 -> 각 특수 방으로 길 파기 (목표 편향 랜덤 워크)
            foreach (var target in specialRoomPos)
            {
                CreateBiasedRoad(start, target);
            }

            // 4) 프리팹 생성
            SpawnRooms();

            // 5) 방 합치기 
            AnnexRooms();
        }

        private void AnnexRooms()
        {
            for(int y =  0; y < mapHeight; y++)
            {
                for(int x = 0; x < mapWidth; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (IsSpecial(pos)) continue;
                    if (mapArray[y, x].type != RoomType.Battle) continue;
                    // 우측/하단이랑 합칠 수 있는지 검사
                    Vector2Int a = pos;
                    Vector2Int b = pos + new Vector2Int(1, 1);

                    bool canAnnexRight = InBounds(a) &&
                        mapArray[y, x].right == WallState.Open &&
                        mapArray[y, x].type == RoomType.Battle &&
                        mapArray[a.y, a.x].type == RoomType.Battle;
                    bool canAnnexDown = InBounds(a) &&
                        mapArray[y, x].down == WallState.Open &&
                        mapArray[y, x].type == RoomType.Battle &&
                        mapArray[a.y, a.x].type == RoomType.Battle;
                    bool canAnnexLeft = InBounds(b) &&
                        mapArray[b.y, b.x].left == WallState.Open &&
                        mapArray[b.y, b.x].type == RoomType.Battle &&
                        mapArray[b.y, b.x].type == RoomType.Battle;
                    bool canAnnexUp = InBounds(b) && 
                        mapArray[b.y, b.x].up == WallState.Open &&
                        mapArray[b.y, b.x].type == RoomType.Battle &&
                        mapArray[b.y, b.x].type == RoomType.Battle;
                    if (canAnnexRight && canAnnexDown && canAnnexLeft && canAnnexUp && maxBigRoomCount > 0)
                    {
                        // 우측 합치기
                        SetRoomType(a, RoomType.Battle);
                        Debug.Log(mapArray[y, x].Object);
                        mapArray[y, x].Object.transform.GetChild(1).gameObject.SetActive(false);
                        mapArray[y, x].Object.transform.GetChild(4).gameObject.SetActive(false);
                        
                        SetRoomType(a + Vector2Int.right, RoomType.Battle);
                        mapArray[y, x+1].Object.transform.GetChild(2).gameObject.SetActive(false);
                        mapArray[y, x+1].Object.transform.GetChild(4).gameObject.SetActive(false);

                        SetRoomType(a + Vector2Int.up, RoomType.Battle);
                        mapArray[y+1, x].Object.transform.GetChild(1).gameObject.SetActive(false);
                        mapArray[y+1, x].Object.transform.GetChild(3).gameObject.SetActive(false);

                        SetRoomType(b, RoomType.Battle);
                        mapArray[b.y, b.x].Object.transform.GetChild(2).gameObject.SetActive(false);
                        mapArray[b.y, b.x].Object.transform.GetChild(3).gameObject.SetActive(false);

                        maxBigRoomCount--;
                    }
                   
                }
            }
        }

        private Vector2Int RandomEmptyCellExcept(Vector2Int except)
        {
            Vector2Int pos;
            int safety = 0;

            do
            {
                pos = new Vector2Int(
                    Random.Range(0, mapWidth),
                    Random.Range(0, mapHeight)
                );

                safety++;
                if (safety > 5000)
                {
                    Debug.LogError("맵에 빈 칸이 부족하거나 루프가 비정상입니다.");
                    break;
                }

            } while (pos == except || IsSpecial(pos) || mapArray[pos.y, pos.x].type != RoomType.Empty);

            return pos;
        }

        private bool IsSpecial(Vector2Int p)
        {
            return specialRoomPos != null && specialRoomPos.Contains(p);
        }

        private bool InBounds(Vector2Int p)
        {
            return p.x >= 0 && p.x < mapWidth && p.y >= 0 && p.y < mapHeight;
        }

        private void SetRoomType(Vector2Int p, RoomType t)
        {
            if (!InBounds(p)) return;

            var info = mapArray[p.y, p.x];
            info.type = t;
            mapArray[p.y, p.x] = info;
        }

        /// <summary>
        /// start -> target까지 "구불구불하지만 도착은 보장"하는 길 생성
        /// </summary>
        private void CreateBiasedRoad(Vector2Int start, Vector2Int target)
        {
            Vector2Int pos = start;
            Vector2Int prevDir = Vector2Int.zero;

            int maxSteps = mapWidth * mapHeight * maxStepsMultiplier;

            // 경로가 너무 길어지는 걸 막고, 도착 못 하면 마지막엔 강제 직진(맨해튼)으로 마무리
            for (int step = 0; step < maxSteps && pos != target; step++)
            {
                Vector2Int dir = ChooseBiasedDir(pos, target, prevDir);
                Vector2Int next = pos + dir;

                if (!InBounds(next))
                    continue;

                // 연결(문 오픈) - "이전 방의 dir"과 "다음 방의 반대"를 연다
                CarveConnection(pos, next, dir);

                // 다음으로 이동
                pos = next;
                prevDir = dir;

                // 특수방/시작방은 타입을 덮어쓰지 않음
                var curType = mapArray[pos.y, pos.x].type;
                if (curType == RoomType.Empty)
                    SetRoomType(pos, RoomType.Battle);
            }

            // 안전장치: 아직 도착 못 했으면 직선으로 마무리
            if (pos != target)
            {
                ForceCarveManhattan(pos, target);
            }
        }

        /// <summary>
        /// 목표로 향하는 방향에 가중치를 주고, 직전 반대방향(backtrack)을 약하게 만드는 방향 선택
        /// </summary>
        private Vector2Int ChooseBiasedDir(Vector2Int pos, Vector2Int target, Vector2Int prevDir)
        {
            // 후보 방향 수집
            List<Vector2Int> candidates = new List<Vector2Int>(4);
            foreach (var d in Dir4)
            {
                Vector2Int next = pos + d;
                if (!InBounds(next)) continue;
                candidates.Add(d);
            }

            // 후보가 비면 그냥 0
            if (candidates.Count == 0)
                return Vector2Int.zero;

            // 목표로 가까워지는 방향 계산(맨해튼 기준)
            int curDist = Manhattan(pos, target);

            // 가중치 계산
            // - 목표에 가까워지는 방향: +towardTargetBias
            // - 반대 방향(-prevDir): 가중치 감소
            float total = 0f;
            float[] w = new float[candidates.Count];

            for (int i = 0; i < candidates.Count; i++)
            {
                var d = candidates[i];
                var next = pos + d;

                float weight = 1f;

                int nextDist = Manhattan(next, target);
                bool closer = nextDist < curDist;

                // 목표 편향
                if (closer) weight += towardTargetBias;

                // 되돌아가기 억제 (단, 막혀있을 수 있으니 0으로 만들진 않음)
                if (prevDir != Vector2Int.zero && d == -prevDir)
                    weight *= (1f - noBacktrackBias); // e.g. 0.15

                // 이미 파인 길(방)으로 가는 건 약간 선호 낮추기(빙빙 방지)
                var t = mapArray[next.y, next.x].type;
                if (t != RoomType.Empty && t != RoomType.Boss && t != RoomType.Start)
                    weight *= 0.75f;

                w[i] = Mathf.Max(0.01f, weight);
                total += w[i];
            }

            // weighted random
            float r = Random.value * total;
            for (int i = 0; i < w.Length; i++)
            {
                r -= w[i];
                if (r <= 0f)
                    return candidates[i];
            }

            return candidates[candidates.Count - 1];
        }

        private int Manhattan(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        /// <summary>
        /// 문(벽) 연결을 정확히 열어준다.
        /// from -> to 로 이동한 dir 기준으로 from의 dir, to의 반대 dir을 Open.
        /// </summary>
        private void CarveConnection(Vector2Int from, Vector2Int to, Vector2Int dir)
        {
            // from 수정
            var a = mapArray[from.y, from.x];
            // to 수정
            var b = mapArray[to.y, to.x];

            if (dir == Vector2Int.up)
            {
                a.down = WallState.Open;
                b.up = WallState.Open;
            }
            else if (dir == Vector2Int.down)
            {
                a.up = WallState.Open;
                b.down = WallState.Open;
            }
            else if (dir == Vector2Int.left)
            {
                a.left = WallState.Open;
                b.right = WallState.Open;
            }
            else if (dir == Vector2Int.right)
            {
                a.right = WallState.Open;
                b.left = WallState.Open;
            }

            mapArray[from.y, from.x] = a;
            mapArray[to.y, to.x] = b;
        }

        /// <summary>
        /// 마지막 안전장치: 목표까지 맨해튼 직선으로 강제로 연결
        /// </summary>
        private void ForceCarveManhattan(Vector2Int from, Vector2Int target)
        {
            Vector2Int pos = from;
            int safety = mapWidth * mapHeight * 2;

            while (pos != target && safety-- > 0)
            {
                Vector2Int dir;

                int dx = target.x - pos.x;
                int dy = target.y - pos.y;

                // 가로/세로 중 하나를 선택해서 직진
                if (Mathf.Abs(dx) > Mathf.Abs(dy))
                    dir = dx > 0 ? Vector2Int.right : Vector2Int.left;
                else
                    dir = dy > 0 ? Vector2Int.up : Vector2Int.down;

                Vector2Int next = pos + dir;
                if (!InBounds(next)) break;

                CarveConnection(pos, next, dir);

                pos = next;

                var curType = mapArray[pos.y, pos.x].type;
                if (curType == RoomType.Empty)
                    SetRoomType(pos, RoomType.Battle);
            }
        }

        private void SpawnRooms()
        {
            // 기존 생성된 Room 오브젝트 정리(에디터에서 여러 번 돌릴 때)
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    RoomInfo info = mapArray[y, x];
                    if (info.type == RoomType.Empty) continue;

                    Room room;

                    // 네 기존 규칙 유지:
                    // Battle/Start는 roomPrefabs[0], 그 외는 roomPrefabs[5]
                    if (info.type == RoomType.Battle || info.type == RoomType.Start)
                    {
                        room = Instantiate(roomPrefabs[0],
                            new Vector3(x * roomPaddingX, y * roomPaddingY, 0),
                            Quaternion.identity, transform).GetComponent<Room>();
                        info.Object = room.gameObject;
                    }
                    else
                    {
                        room = Instantiate(roomPrefabs[5],
                            new Vector3(x * roomPaddingX, y * roomPaddingY, 0),
                            Quaternion.identity, transform).GetComponent<Room>();
                        
                    }

                    mapArray[y, x] = info;

                    // 벽 프리팹 인덱싱도 네 코드 유지
                    GameObject up = roomPrefabs[3 + (int)info.up];
                    GameObject down = roomPrefabs[3 + (int)info.down];
                    GameObject left = roomPrefabs[1 + (int)info.left];
                    GameObject right = roomPrefabs[1 + (int)info.right];

                    room.CreateWall(up, down, left, right);

                    // 정렬도 네 방식 유지
                    room.SetOrderInLayer((mapWidth - x) * (1 + y));
                }
            }
        }
    }
}
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CSH.Scripts.Maps
{
    public class MapLoader : MonoBehaviour
    {
        [Header("Tilemap")]
        [SerializeField] private Tilemap background;
        [SerializeField] private Tilemap wall;
        [SerializeField] private Tilemap design;

        [Header("Tiles")]
        [SerializeField] private TileBase wallTile;
        [SerializeField] private TileBase groundTile;
        [SerializeField] private TileBase treeTile;

        [Header("Size")]
        [SerializeField] private int width = 128;
        [SerializeField] private int height = 128;

        [Header("Seed")]
        [SerializeField] private int seed = 12345;

        [Header("Noise (Height)")]
        [SerializeField] private float heightScale = 0.04f;
        [SerializeField] private int heightOctaves = 5;
        [SerializeField] private float heightPersistence = 0.55f;
        [SerializeField] private float heightLacunarity = 2.0f;

        [Header("Biome Thresholds")]
        [SerializeField] private float wallThreshold;
        [SerializeField] private float groundThreshold = 0.38f;

        [Header("Island Falloff")]
        [SerializeField] private bool useFalloff = true;
        [SerializeField] private float falloffStrength = 1.25f; // 높을수록 가장자리 물이 많아짐

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (!background && !wall)
            {
                Debug.LogError("Tilemap reference missing.");
                return;
            }

            seed = Random.Range(1, int.MaxValue);

            background.ClearAllTiles();
            wall.ClearAllTiles();
            design.ClearAllTiles();

            // 1) 노이즈 필드 생성
            float[,] hMap = new float[height, width];

            // seed 기반 offset
            System.Random prng = new System.Random(seed);
            float hOx = prng.Next(-100000, 100000);
            float hOy = prng.Next(-100000, 100000);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float h = FractalPerlin01(x, y, hOx, hOy, heightScale, heightOctaves, heightPersistence, heightLacunarity);
                    

                    if (useFalloff)
                        h = Mathf.Clamp01(h - Falloff01(x, y) * falloffStrength);

                    hMap[y, x] = h;
                }

            // 2) (선택) 연결성 보정: "땅"을 가장 큰 덩어리만 남기기
            //    - 여기서는 wallThreshold 기준으로 ground를 정의
            KeepLargestLandMass(hMap, wallThreshold);

            // 3) 타일 배치
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float h = hMap[y, x];
                    //Debug.Log($"h[{x},{y}] = {h}");
                    TileBase tile = PickBiomeTile(h);
                    
                    if (tile == groundTile)
                    {
                        background.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                    else
                    {
                        wall.SetTile(new Vector3Int(x, y, 0), wallTile);
                        design.SetTile(new Vector3Int(x, y, 0), treeTile);
                    }
                }
            }
            for(int y = -1; y <= height; y++)
            {
                wall.SetTile(new Vector3Int(-1, y, 0), groundTile);
                wall.SetTile(new Vector3Int(width, y, 0), groundTile);
            }
            for(int x = -1; x <= width; x++)
            {
                wall.SetTile(new Vector3Int(x, -1, 0), groundTile);
                wall.SetTile(new Vector3Int(x, height, 0), groundTile);

            }
        }

        private TileBase PickBiomeTile(float h)
        {
            Debug.Log(h < wallThreshold); 
            if (h < wallThreshold) 
                return treeTile;
            else
                return groundTile;
        }

        private float FractalPerlin01(int x, int y, float ox, float oy, float scale, int octaves, float persistence, float lacunarity)
        {
            float amp = 1f;
            float freq = 1f;
            float value = 0f;
            float max = 0f;

            for (int i = 0; i < octaves; i++)
            {
                float sx = (x + ox) * scale * freq;
                float sy = (y + oy) * scale * freq;

                value += Mathf.PerlinNoise(sx, sy) * amp;
                max += amp;

                amp *= persistence;
                freq *= lacunarity;
            }

            return (max > 0f) ? (value / max) : 0f;
        }

        // 0(중앙) -> 1(가장자리)로 증가하는 falloff
        private float Falloff01(int x, int y)
        {
            float nx = (x / (float)(width - 1)) * 2f - 1f;
            float ny = (y / (float)(height - 1)) * 2f - 1f;

            // 원형에 가까운 falloff
            float d = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
            // 부드럽게
            return d * d;
        }

        // Land(=h >= wallThreshold) 영역에서 가장 큰 연결 덩어리만 남기고 나머지는 Wall로 바꿈
        private void KeepLargestLandMass(float[,] hMap, float wallT)
        {
            bool[,] isLand = new bool[height, width];
            bool[,] visited = new bool[height, width];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    isLand[y, x] = hMap[y, x] >= wallT;

            List<Vector2Int> best = null;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (visited[y, x] || !isLand[y, x]) continue;

                    var comp = FloodCollect(isLand, visited, x, y);
                    if (best == null || comp.Count > best.Count)
                        best = comp;
                }

            if (best == null) return;

            // best가 아닌 ground는 Wall로 강제
            bool[,] keep = new bool[height, width];
            foreach (var p in best)
                keep[p.y, p.x] = true;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (isLand[y, x] && !keep[y, x])
                        hMap[y, x] = 0f; // 벽으로
                }
        }

        private List<Vector2Int> FloodCollect(bool[,] isLand, bool[,] visited, int sx, int sy)
        {
            var q = new Queue<Vector2Int>();
            var comp = new List<Vector2Int>();

            q.Enqueue(new Vector2Int(sx, sy));
            visited[sy, sx] = true;
            
            while (q.Count > 0)
            {
                var p = q.Dequeue();
                comp.Add(p);

                // 4방 연결
                TryEnqueue(p.x + 1, p.y);
                TryEnqueue(p.x - 1, p.y);
                TryEnqueue(p.x, p.y + 1);
                TryEnqueue(p.x, p.y - 1);
            }

            return comp;

            void TryEnqueue(int x, int y)
            {
                if (x < 0 || x >= width || y < 0 || y >= height) return;
                if (visited[y, x]) return;
                if (!isLand[y, x]) return;

                visited[y, x] = true;
                q.Enqueue(new Vector2Int(x, y));
            }
        }
    }
}