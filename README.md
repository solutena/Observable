# 설치

![image](https://github.com/user-attachments/assets/97fd85b1-10d7-48b1-a496-77d26d90dbe9)

1. URL 복사

![image](https://github.com/user-attachments/assets/f4060f1d-94aa-4a49-b001-e7a5e01316e1)

2. 패키지 매니저에서 Add Package from Git URL 선택
   
![image](https://github.com/user-attachments/assets/1ada1140-2c98-4227-87fb-13fd64c693ef)

3.  복사한 URL로 설치

# Observable<T>

`Observable<T>`는 제네릭 클래스로, 옵저버 패턴을 구현하여 값이 변경될 때 구독자들에게 알림을 보내는 기능을 제공합니다.

## 이벤트
- **OnChanged:** 값이 변경될 때마다 호출됩니다. 이를 통해 값이 변경될 때 특정 동작을 수행할 수 있습니다.

## 사용법

직렬화 및 역직렬화 시 데이터를 적절히 처리하는 방법을 보여주는 예제입니다.

```csharp
using System;
using UnityEngine;

[Serializable]
public class Example : ISerializationCallbackReceiver
{
	[SerializeField] int hp;

	public Observable<int> HP { get; set; }

	public void OnAfterDeserialize()
	{
		hp = HP;
	}

	public void OnBeforeSerialize()
	{
		HP = new Observable<int>(hp);
	}
}
```
### 이벤트 구독

```csharp
Hp.OnChanged += (int value) =>
{
  Debug.Log($"값 변경됨: {value}");
};
```

# ObservableCollection<T>

`ObservableCollection<T>`는 컬렉션에 항목이 추가, 제거 또는 변경될 때 구독자에게 알림을 보내는 기능을 제공합니다.

## 기능
- **Add:** 항목을 컬렉션에 추가하고 `OnAdd` 이벤트를 발생시킵니다. 후에 `OnChanged` 이벤트를 발생시킵니다.
- **Remove:** 항목을 컬렉션에서 제거하고 `OnRemove` 이벤트를 발생시킵니다. 후에 `OnChanged` 이벤트를 발생시킵니다.
- **Clear:** 모든 항목을 삭제하고 `OnRemove` 이벤트를 개별 항목에 대해 발생시킵니다. 후에 `OnChanged` 이벤트를 발생시킵니다.
- **Refresh:** 컬렉션을 다시 알림 상태로 만들어 강제로 `OnChanged` 이벤트를 발생시킵니다.

## 이벤트
- **OnAdd:** 항목이 추가될 때 발생하는 이벤트입니다. `Action<T>` 형식으로 추가된 항목을 매개변수로 전달합니다.
- **OnRemove:** 항목이 삭제될 때 발생하는 이벤트입니다. `Action<T>` 형식으로 삭제된 항목을 매개변수로 전달합니다.
- **OnChanged:** 컬렉션에 변경이 생길 때마다 호출되는 이벤트입니다. `Action<ObservableCollection<T>>` 형식으로 전체 컬렉션을 매개변수로 전달합니다.

## 사용법

`ObservableCollection<T>` 클래스는 `ICollection<T>`나 `IEnumerable<T>`로 초기화할 수 있습니다.

```csharp
List<int> list = new();
HashSet<int> hashset = new();
Dictionary<int, int> dictionary = new();

var observableList = new ObservableCollection<int>(list);
var observableHashSet = new ObservableCollection<int>(hashset);
var observableDictionary = new ObservableCollection<KeyValuePair<int, int>>(dictionary);
```

직렬화 및 역직렬화 시 데이터를 적절히 처리하는 방법을 보여주는 예제입니다.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Example : ISerializationCallbackReceiver
{
	[SerializeField] List<int> list;

	public ObservableCollection<int> List { get; set; }

	public void OnAfterDeserialize()
	{
		list = List.ToList();
	}

	public void OnBeforeSerialize()
	{
		List = new ObservableCollection<int>(list);
	}
}
```

### 이벤트 구독
```csharp
List.OnAdd += item => Debug.Log($"항목 추가됨: {item}");
List.OnRemove += item => Debug.Log($"항목 제거됨: {item}");
List.OnChanged += collection => Debug.Log("컬렉션이 변경되었습니다.");
```
