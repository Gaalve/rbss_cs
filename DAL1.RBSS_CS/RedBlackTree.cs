using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL1.RBSS_CS
{
    

    
    
    public class RedBlackTree<T> where T : IComparable<T>
    {
        enum NodeColor
        {
            Black,
            Red
        }
        enum Direction
        {
            Left,
            Right
        }
        private class TreeNode<T> where T : IComparable<T>
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

        private TreeNode<T>? root;

        private TreeNode<T> getMaximum(TreeNode<T> node)
        {
            while (node.RightChild != null)
            {
                node = node.RightChild;
            }

            return node;
        }

        private TreeNode<T> getMinimum(TreeNode<T> node)
        {
            while (node.LeftChild != null)
            {
                node = node.LeftChild;
            }

            return node;
        }

        private TreeNode<T>? getSuccessor(TreeNode<T> node)
        {
            if (node.RightChild != null) return getMinimum(node.RightChild);
            var parent = node.Parent;
            while (parent != null && node == parent.RightChild)
            {
                node = parent;
                parent = node.Parent;
            }

            return parent;
        }

        private TreeNode<T>? getPredecessor(TreeNode<T> node)
        {
            if (node.LeftChild != null) return getMaximum(node.LeftChild);
            var parent = node.Parent;
            while (parent != null && node == parent.LeftChild)
            {
                node = parent;
                parent = node.Parent;
            }

            return parent;
        }

        private TreeNode<T> RotateRootLeft(TreeNode<T> node)
        {
            var parent = node.Parent;
            var nextRoot = node.RightChild;
            var nextChild = node.LeftChild;
            node.RightChild = nextChild;
            if (nextChild != null) nextChild.Parent = node;
            nextRoot!.LeftChild = node;
            node.Parent = nextRoot;
            nextRoot.Parent = parent;
            if (parent != null)
            {
                if (node == parent.RightChild) parent.RightChild = nextRoot;
                else parent.LeftChild = nextRoot;
            }
            else
            {
                root = nextRoot;
            }
            return nextRoot;
        }

        private TreeNode<T> RotateRootRight(TreeNode<T> node)
        {
            var parent = node.Parent;
            var nextRoot = node.LeftChild;
            var nextChild = node.RightChild;
            node.LeftChild = nextChild;
            if (nextChild != null) nextChild.Parent = node;
            nextRoot!.RightChild = node;
            node.Parent = nextRoot;
            nextRoot.Parent = parent;
            if (parent != null)
            {
                if (node == parent.RightChild) parent.RightChild = nextRoot;
                else parent.LeftChild = nextRoot;
            }
            else
            {
                root = nextRoot;
            }
            return nextRoot;
        }


        private Direction getOpposite(Direction dir)
        {
            return dir == Direction.Left ? Direction.Right : Direction.Left;
        }

        private ref TreeNode<T>? getChild(TreeNode<T> node, Direction dir)
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
                root = newNode;
                return;
            }

            getChild(parent, dir) = newNode;

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
                var uncle = getChild(grandParent, getOpposite(dir));

                if (uncle == null || uncle.Color == NodeColor.Black)
                {
                    if (newNode == getChild(parent, getOpposite(dir)))
                    {
                        if (dir == Direction.Left) RotateRootLeft(parent);
                        else RotateRootRight(parent);
                        // newNode = parent;
                        parent = getChild(grandParent, dir);
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
            if (node == root)
            {
                if (node.LeftChild == null && node.RightChild == null)
                {
                    root = null;
                    return;
                }

                if (node.LeftChild != null && node.RightChild != null)
                {
                    //TODO
                    return;
                }

                if (node.LeftChild != null)
                {
                    root = node.LeftChild;
                    root.Parent = null;
                    root.Color = NodeColor.Black;
                }
                else
                {
                    root = node.RightChild;
                    root!.Parent = null;
                    root.Color = NodeColor.Black;
                }

                return;
            }

            var parent = node.Parent;
            var dir = parent!.LeftChild == node ? Direction.Left : Direction.Right;

            getChild(parent, dir) = null;


        }


        private TreeNode<T>? search(TreeNode<T>? node, T key)
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
            var node = search(root, key);
            return node == null ? default : node.Data;
        }

        private List<T> getSortedList(List<T> list, TreeNode<T>? node)
        {
            if (node != null)
            {
                getSortedList(list, node.LeftChild);
                list.Add(node.Data);
                getSortedList(list, node.RightChild);

            }

            return list;
        }
        public List<T> GetSortedList()
        {
            return getSortedList(new List<T>(), root);
        }

        public void Insert(T key)
        {
            TreeNode<T> keyNode = new TreeNode<T>(key);
            TreeNode<T>? parent = null;
            TreeNode<T>? current = root;

            while (current != null)
            {
                parent = current;
                var comp = keyNode.Data.CompareTo(current.Data);
                if (comp == 0) return;
                if (comp < 0) current = current.LeftChild;
                else current = current.RightChild;
            }

            if (parent == null) Insert(keyNode, parent, Direction.Left);
            else Insert(keyNode, parent, key.CompareTo(parent.Data) < 0 ? Direction.Left : Direction.Right);
        }

        private int getHeight(TreeNode<T>? node, int h)
        {
            if (node == null) return h;
            h += 1;
            return Math.Max(getHeight(node.LeftChild, h), getHeight(node.RightChild, h));
        }

        public int GetHeight()
        {
            return getHeight(root, 0);
        }

        public void Clear()
        {
            root = null;
        }
    }


}
