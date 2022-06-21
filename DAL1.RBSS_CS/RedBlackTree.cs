namespace DAL1.RBSS_CS
{
    

    
    
    public class RedBlackTree<T> where T : PrecalculatedHash, IComparable<T>
    {
        private enum NodeColor
        {
            Black,
            Red
        }

        private enum Direction
        {
            Left,
            Right
        }
        private class TreeNode<T> where T : PrecalculatedHash, IComparable<T>
        {
            public TreeNode<T>? Parent;
            public TreeNode<T>? LeftChild;
            public TreeNode<T>? RightChild;

            public NodeColor Color;

            public readonly T Data;

            public TreeNode(T data)
            {
                Parent = null;
                LeftChild = null;
                RightChild = null;
                Color = NodeColor.Red;
                Data = data;
            }

        }

        private TreeNode<T>? _root;

        private TreeNode<T> GetMaximum(TreeNode<T> node)
        {
            while (node.RightChild != null)
            {
                node = node.RightChild;
            }

            return node;
        }

        private TreeNode<T> GetMinimum(TreeNode<T> node)
        {
            while (node.LeftChild != null)
            {
                node = node.LeftChild;
            }

            return node;
        }

        private TreeNode<T>? GetSuccessor(TreeNode<T> node)
        {
            if (node.RightChild != null) return GetMinimum(node.RightChild);
            var parent = node.Parent;
            while (parent != null && node == parent.RightChild)
            {
                node = parent;
                parent = node.Parent;
            }

            return parent;
        }

        private TreeNode<T>? GetPredecessor(TreeNode<T> node)
        {
            if (node.LeftChild != null) return GetMaximum(node.LeftChild);
            var parent = node.Parent;
            while (parent != null && node == parent.LeftChild)
            {
                node = parent;
                parent = node.Parent;
            }

            return parent;
        }

        private TreeNode<T> RotateRootLeft(TreeNode<T> parent)
        {
            var grandParent = parent.Parent;
            var sibling = parent.RightChild;
            var nextChild = sibling!.LeftChild;
            parent.RightChild = nextChild;
            if (nextChild != null) nextChild.Parent = parent;
            sibling.LeftChild = parent;
            parent.Parent = sibling;
            sibling.Parent = grandParent;
            if (grandParent != null)
            {
                if (parent == grandParent.RightChild) grandParent.RightChild = sibling;
                else grandParent.LeftChild = sibling;
            }
            else
            {
                _root = sibling;
            }
            return sibling;
        }

        private TreeNode<T> RotateRootRight(TreeNode<T> parent)
        {
            var grandParent = parent.Parent;
            var sibling = parent.LeftChild;
            var nextChild = sibling!.RightChild;
            parent.LeftChild = nextChild;
            if (nextChild != null) nextChild.Parent = parent;
            sibling.RightChild = parent;
            parent.Parent = sibling;
            sibling.Parent = grandParent;
            if (grandParent != null)
            {
                if (parent == grandParent.RightChild) grandParent.RightChild = sibling;
                else grandParent.LeftChild = sibling;
            }
            else
            {
                _root = sibling;
            }
            return sibling;
        }


        private static Direction GetOpposite(Direction dir)
        {
            return dir == Direction.Left ? Direction.Right : Direction.Left;
        }

        private static ref TreeNode<T>? GetChild(TreeNode<T> node, Direction dir)
        {
            if (dir == Direction.Left) return ref node.LeftChild;
            return ref node.RightChild;
        }

        private void Insert(TreeNode<T> newNode, TreeNode<T>? parent, Direction dir)
        {
            newNode.Color = NodeColor.Red;
            newNode.LeftChild = null;
            newNode.RightChild = null;
            newNode.Parent = parent;

            if (parent == null)
            {
                _root = newNode;
                return;
            }

            GetChild(parent, dir) = newNode;

            do
            {
                if (parent.Color == NodeColor.Black) return;
                var grandParent = parent.Parent;
                if (grandParent == null)
                {
                    parent.Color = NodeColor.Black;
                    return;
                }

                dir = grandParent.LeftChild == parent ? Direction.Left : Direction.Right;
                var uncle = GetChild(grandParent, GetOpposite(dir));

                if (uncle == null || uncle.Color == NodeColor.Black)
                {
                    if (newNode == GetChild(parent, GetOpposite(dir)))
                    {
                        if (dir == Direction.Left) RotateRootLeft(parent);
                        else RotateRootRight(parent);
                        // newNode = parent;
                        parent = GetChild(grandParent, dir);
                    }

                    if (dir == Direction.Left) RotateRootRight(grandParent);
                    else RotateRootLeft(grandParent);

                    parent!.Color = NodeColor.Black;
                    grandParent.Color = NodeColor.Red;

                    return;
                }

                parent.Color = NodeColor.Black;
                uncle.Color = NodeColor.Black;
                grandParent.Color = NodeColor.Red;

                newNode = grandParent;

            } while ((parent = newNode.Parent) != null);
        }

        private void Delete(TreeNode<T> node)
        {
            //TODO
            if (node == _root)
            {
                if (node.LeftChild == null && node.RightChild == null)
                {
                    _root = null;
                    return;
                }

                if (node.LeftChild != null && node.RightChild != null)
                {
                    //TODO
                    return;
                }

                if (node.LeftChild != null)
                {
                    _root = node.LeftChild;
                    _root.Parent = null;
                    _root.Color = NodeColor.Black;
                }
                else
                {
                    _root = node.RightChild;
                    _root!.Parent = null;
                    _root.Color = NodeColor.Black;
                }

                return;
            }

            var parent = node.Parent;
            var dir = parent!.LeftChild == node ? Direction.Left : Direction.Right;

            GetChild(parent, dir) = null;


        }

        /// <summary>
        /// Find the node within with [x, y)S that is closest to the root, assuming x is less than y 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private TreeNode<T>? FindInitial(T x, T y, bool enableRightBoundCheck)
        {
            TreeNode<T>? node = _root;
            while (node != null)
            {
                if (node.Data.CompareTo(x) < 0)
                {
                    node = node.RightChild;
                }
                else if (enableRightBoundCheck && node.Data.CompareTo(y) >= 0)
                {
                    node = node.LeftChild;
                }
                else break;
            }

            return node;
        }


        private int GetFingerprint(TreeNode<T>? node, T x, T y)
        {
            if (node == null) return 0;
            if (node.Data.CompareTo(x) < 0) return 0;
            if (node.Data.CompareTo(y) >= 0) return 0;
            return node.Data.Hash ^ GetFingerprint(node.LeftChild, x, y) ^
                   GetFingerprint(node.RightChild, x, y);
        }

        /// <summary>
        /// Calculates Hash within the range [x, y) via bifunctor xor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetFingerprint(T x, T y)
        {
            var enableRightBoundCheck = x.CompareTo(y) < 0;
            TreeNode<T>? node = FindInitial(x, y, enableRightBoundCheck);
            return GetFingerprint(node, x, y);
        }
        private static List<T> GetSortedListBetween(List<T> list, TreeNode<T>? node, T x, T y, bool enableRightBoundCheck)
        {
            if (node == null || node.Data.CompareTo(x) < 0 || (enableRightBoundCheck && node.Data.CompareTo(y) >= 0)) return list;
            GetSortedListBetween(list, node.LeftChild, x, y, enableRightBoundCheck);
            list.Add(node.Data);
            GetSortedListBetween(list, node.RightChild, x, y, enableRightBoundCheck);

            return list;
        }
        public List<T> GetSortedListBetween(T x, T y)
        {
            var enableRightBoundCheck = x.CompareTo(y) < 0;
            var node = FindInitial(x, y, enableRightBoundCheck);
            return GetSortedListBetween(new List<T>(), node, x, y, enableRightBoundCheck);
        }

        private static TreeNode<T>? Search(TreeNode<T>? node, T key)
        {
            int comp;
            while (node != null && (comp = key.CompareTo(node.Data)) != 0)
            {
                node = comp < 0 ? node.LeftChild : node.RightChild;
            }

            return node;
        }

        public T? Search(T key)
        {
            var node = Search(_root, key);
            return node?.Data;
        }

        private static List<T> GetSortedList(List<T> list, TreeNode<T>? node)
        {
            if (node == null) return list;
            GetSortedList(list, node.LeftChild);
            list.Add(node.Data);
            GetSortedList(list, node.RightChild);

            return list;
        }
        public List<T> GetSortedList()
        {
            return GetSortedList(new List<T>(), _root);
        }

        public bool Insert(T key)
        {
            TreeNode<T> keyNode = new TreeNode<T>(key);
            TreeNode<T>? parent = null;
            TreeNode<T>? current = _root;

            while (current != null)
            {
                parent = current;
                var comp = keyNode.Data.CompareTo(current.Data);
                if (comp == 0) return false;
                if (comp < 0) current = current.LeftChild;
                else current = current.RightChild;
            }

            if (parent == null) Insert(keyNode, parent, Direction.Left);
            else Insert(keyNode, parent, key.CompareTo(parent.Data) < 0 ? Direction.Left : Direction.Right);
            return true;
        }

        private static int GetHeight(TreeNode<T>? node, int h)
        {
            if (node == null) return h;
            h += 1;
            return Math.Max(GetHeight(node.LeftChild, h), GetHeight(node.RightChild, h));
        }

        public int GetHeight()
        {
            return GetHeight(_root, 0);
        }

        public void Clear()
        {
            _root = null;
        }
    }


}
