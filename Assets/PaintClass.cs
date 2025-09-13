using System;
using BrunoMikoski.TextJuicer;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintClass : MonoBehaviour
{
    
    public static PaintClass Instance;
    
    public Camera cam;
    public Image paintImage;
    public int radius;
    public Color paintColor;
    public Button b_Clear;
    public Button b_Send;

    private Texture2D _texture;
    private Sprite _sprite;
    private bool _isPainted = false;
    private Vector2 _vector1;
    private Vector2 _vector2;
    private Vector2 _lastPosition;
    private bool isDragging = false;
    private Image _circle;
    private TMP_Text _sendText;
    
    RaycastHit hit;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    void Start()
    {
        b_Clear.onClick.AddListener(() => ClearTexture(Color.clear));
        b_Send.onClick.AddListener(OnSend);
        _texture = new Texture2D((int)paintImage.rectTransform.rect.width,
            (int)paintImage.rectTransform.rect.height, TextureFormat.RGBA32, false);
        _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), Vector2.zero);
        paintImage.sprite = _sprite;
        ClearTexture(Color.clear);
        _circle = GameManager.instance.Send.GetComponentInChildren<Image>();
        _sendText = GameManager.instance.Send.GetComponentInChildren<TMP_Text>();
        _circle.color = new Color(1f, 1f, 1f, 0f);
        _sendText.color = new Color(_sendText.color.r, _sendText.color.g, _sendText.color.b, 0f);
    }

    
    void Update()
    {
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = cam.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0; // Для 2D, где z=0
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _lastPosition = touch.position;
                    isDragging = true;

                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Ray ray = cam.ScreenPointToRay(touch.position);
                        //text.text = "Border";
                        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Border"))
                        {
                            OnPaint((int)(_lastPosition.x - paintImage.rectTransform.offsetMin.x),
                                (int)(_lastPosition.y - paintImage.rectTransform.offsetMin.y),
                                (int)(touch.position.x - paintImage.rectTransform.offsetMin.x),
                                (int)(touch.position.y - paintImage.rectTransform.offsetMin.y));

                            _lastPosition = touch.position;
                        }

                    }

                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isPainted = false;
        }
        
    }

    private void OnSend()
    {
        b_Clear.enabled = false;
        b_Send.enabled = false;
        GameManager.instance.AddCountSend();
        _circle.DOFade(1f, 0.5f);
        _sendText.DOFade(1f, 0.5f);
        _circle.DOFade(0, 0.5f).SetDelay(3f);
        _sendText.DOFade(0, 0.5f).SetDelay(3f).OnComplete(FinishSend);
        
    }

    private void FinishSend()
    {
        ClearTexture(Color.clear);
        GameManager.instance.Send.SetActive(false);
        GameManager.instance.CountSendParent.SetActive(true);
        b_Clear.enabled = true;
        b_Send.enabled = true;
    }

    private void OnPaint(int x1, int y1, int x2, int y2)
    {
        DrawThickLineOptimized(x1, y1, x2, y2, paintColor);
    }

    public void ClearTexture(Color color)
    {
        Color[] pixels = new Color[_texture.width * _texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        _texture.SetPixels(pixels);
        _texture.Apply();
    }
    
    void SetPixelSafe(int x, int y, Color color)
    {
        // Проверка границ текстуры
        if (x >= 0 && x < _texture.width && y >= 0 && y < _texture.height)
        {
            _texture.SetPixel(x, y, color);
        }
    }

    void DrawCircle(int centerX, int centerY, int radius, Color color)
    {
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                _vector1.x = x;
                _vector1.y = y;
                _vector2.x = centerX;
                _vector2.y = centerY;
                float distance = Vector2.Distance(_vector1, _vector2);
                if (distance < radius)
                {
                    SetPixelSafe(x, y, color);
                }
            }
        }

        _texture.Apply();
    }

    public void DrawThickLineOptimized(int x0, int y0, int x1, int y1, Color color)
    {
        // Вычисляем вектор направления линии
        Vector2 lineDir = new Vector2(x1 - x0, y1 - y0).normalized;
        
        // Вычисляем перпендикулярный вектор
        Vector2 normal = new Vector2(-lineDir.y, lineDir.x);
        
        // Вычисляем смещение для толщины
        Vector2 offset = normal * radius;
        
        // Рисуем линию с помощью алгоритма Брезенхэма, расширяя ее в перпендикулярном направлении
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;
        
        while (true)
        {
            // Рисуем перпендикулярный отрезок в каждой точке линии
            DrawPerpendicularLine(x0, y0, offset, color);
            
            if (x0 == x1 && y0 == y1) break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
        
        // Рисуем закругленные концы
        DrawCircle(x0, y0, radius, color);
        DrawCircle(x1, y1, radius, color);
    }
    
    // Рисует перпендикулярный отрезок в точке линии
    void DrawPerpendicularLine(int x, int y, Vector2 offset, Color color)
    {
        int startX = (int)(x - offset.x);
        int startY = (int)(y - offset.y);
        int endX = (int)(x + offset.x);
        int endY = (int)(y + offset.y);
        
        // Рисуем линию от start до end
        DrawLine(startX, startY, endX, endY, color);
        //DrawLine(startX+1, startY+1, endX+1, endY+1, color);
        //DrawLine(startX-1, startY-1, endX-1, endY-1, color);
    }
    
    // Метод для рисования линии (алгоритм Брезенхэма)
    void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;
        
        while (true)
        {
            SetPixelSafe(x0, y0, color);
            SetPixelSafe(x0+1, y0, color);
            
            if (x0 == x1 && y0 == y1) break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    // private void OnMouseDown()
    // {
    //     _isPainted = true;
    //     _lastPosition = Input.mousePosition;
    // }

    // private void OnMouseDrag()
    // {
    //     if (_isPainted)
    //     {
    //         Vector3 viewportPos = cam.ScreenToViewportPoint(Input.mousePosition);
    //         // Если viewportPos находится в пределах [0,1] по x и y, то мышь над этим дисплеем
    //         if (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
    //         {
    //             Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //             if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Border"))
    //             {
    //                 OnPaint((int)(_lastPosition.x - paintImage.rectTransform.offsetMin.x),
    //                     (int)(_lastPosition.y - paintImage.rectTransform.offsetMin.y),
    //                     (int)(Input.mousePosition.x - paintImage.rectTransform.offsetMin.x),
    //                     (int)(Input.mousePosition.y - paintImage.rectTransform.offsetMin.y));
    //             
    //                 _lastPosition = Input.mousePosition;
    //             }
    //         }
    //     }
    // }
}
